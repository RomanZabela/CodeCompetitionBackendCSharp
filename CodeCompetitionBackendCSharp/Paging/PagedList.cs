using CodeCompetitionBackendCSharp.RequestFeatures;

namespace CodeCompetitionBackendCSharp.Paging
{
	public class PagedList<T> : List<T>	
	{
		public MetaData MetaData { get; set; }

		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			MetaData = new MetaData
			{
				TotalCount = count,
				PageSize = pageSize,
				CurrentPage = pageNumber,
				TotalPages = (int)Math.Ceiling(count / (double)pageSize)
			};

			AddRange(items);
		}

		public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize, bool switchedOff = false)
		{
			List<T> items;
			int count = 0;

			if (switchedOff)
			{
                count = source.Count();
				items = source.ToList();
            }
			else
			{
                count = source.Count();
                items = source
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();
            }
			

			return new PagedList<T>(items, count, pageNumber, pageSize);		
		}
	}
}
