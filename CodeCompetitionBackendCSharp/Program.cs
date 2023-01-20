using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Repository;
using CodeCompetitionBackendCSharp.Services;
using CodeCompetitionBackendCSharp.Services.ServiceExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NLog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
builder.Services.ConfigureLoggerService();

// Add services to the container.
builder.Services.AddCors(policy =>
{
	policy.AddPolicy("CorsPolicy", opt => opt
		.WithOrigins("https://localhost:7094")
		.AllowAnyHeader()
		.AllowAnyMethod()
		.WithExposedHeaders("X-Pagination"));
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.Authority = builder.Configuration["Auth0:Domain"];
	options.Audience = builder.Configuration["Auth0:ApiIdentifier"];
});

builder.Services.AddAuth0AuthenticationClient(config =>
{
	config.Domain = builder.Configuration["Auth0:Domain"];
	config.ClientId = builder.Configuration["Auth0:ClientId"];
	config.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
});

builder.Services.AddAuth0ManagementClient().AddManagementAccessToken();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskCategoryService, TaskCategoryService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITaskToCompetitionService, TaskToCompetitionService>();
builder.Services.AddScoped<ITaskToTeamService, TaskToTeamService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
	{
		logger.LogError($"{context.Response.StatusCode.ToString()} Unauthorized access");
	};

});

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")),
    RequestPath = new PathString("/StaticFiles")
});

app.MapControllers();

app.Run();