using CodeCompetitionBackendCSharp.Data;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Repository;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
string domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    policy.WithOrigins("https://localhost:7094")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("X-Pagination"));
});

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:Domain"];
    options.Audience = builder.Configuration["Auth0:ApiIdentifier"];
});

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
