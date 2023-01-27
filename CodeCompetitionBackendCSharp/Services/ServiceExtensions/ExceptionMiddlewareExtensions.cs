using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CodeCompetitionBackendCSharp.Services.ServiceExtensions
{
	public static class ExceptionMiddlewareExtensions
	{
		public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
		{
			app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";

					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						logger.LogError($"Error: {contextFeature.Error}");

						await context.Response.WriteAsync(new ErrorDetails()
						{
							StatusCode = context.Response.StatusCode,
							Message = "Internal Server Error."
						}.ToString());
					}
				});
			});
		}
		public static void ConfigureLoggerService(this IServiceCollection services)
		{
			services.AddSingleton<ILoggerManager, LoggerManager>();
		}
	}
}
