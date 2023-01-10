using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using CodeCompetitionBackendCSharp.Dto;

namespace CodeCompetitionBackendCSharp.Services.ServiceExtensions
{
	public static class ServiceTaskExtentios
	{
		public static IQueryable<TaskWithCategoryNameDto> Search(this IQueryable<TaskWithCategoryNameDto> tasks, string searchString)
		{
			if (string.IsNullOrWhiteSpace(searchString))
			{
				return tasks;
			}

			var lowerCaseSearchString = searchString.Trim().ToLower();

			return tasks.Where(t => t.TaskName.Trim().ToLower().Contains(lowerCaseSearchString));
		}

		public static IQueryable<TaskWithCategoryNameDto> Sort(this IQueryable<TaskWithCategoryNameDto> tasks, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
			{
				return tasks.OrderBy(t => t.TaskName);
			}

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(TaskWithCategoryNameDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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

			return tasks.OrderBy(orderQuery);
		}
	}
}
