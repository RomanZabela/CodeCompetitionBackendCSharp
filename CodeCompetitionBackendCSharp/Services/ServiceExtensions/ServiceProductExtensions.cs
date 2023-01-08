using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace CodeCompetitionBackendCSharp.Services.ServiceExtensions
{
	public static class ServiceProductExtensions
	{
		public static IQueryable<Models.Task> Search(this IQueryable<Models.Task> tasks, string searchString)
		{
			if (string.IsNullOrWhiteSpace(searchString))
			{
				return tasks;
			}

			var lowerCaseSearchString = searchString.Trim().ToLower();

			return tasks.Where(t => t.TaskName.Trim().ToLower().Contains(lowerCaseSearchString));
		}

		public static IQueryable<Models.Task> Sort(this IQueryable<Models.Task> tasks, string orderByQueryString)
		{
			if(string.IsNullOrWhiteSpace(orderByQueryString))
			{
				return tasks.OrderBy(t => t.TaskName);
			}

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(Models.Task).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var orderQueryBuilder = new StringBuilder();

			foreach (var param in orderParams)
			{
				if (string.IsNullOrWhiteSpace(param))
				{
					continue;
				}

				var propertyFromQueryName = param.Split(" ")[0];
				var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

				if (objectProperty == null)
				{
					continue;
				}

				var direction = param.EndsWith(" desc") ? "descending" : "ascending";
				orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
			}

			var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
			if (string.IsNullOrWhiteSpace(orderQuery))
			{
				return tasks.OrderBy(t => t.TaskName);
			}
			Console.WriteLine(orderQuery);
			return tasks.OrderBy(orderQuery);
		}
	}
}
