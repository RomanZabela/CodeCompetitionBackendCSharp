namespace CodeCompetitionBackendCSharp.RequestFeatures
{
	public class TaskToCompetitionParameters
	{
		public bool switchOff = false;
		const int maxPageSize = 50;
		public int PageNumber { get; set; } = 1;

		public string? SearchString { get; set; } = string.Empty;
		public string? OrderBy { get; set; } = string.Empty;

		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
		}

		private int _pageSize = 6;
	}
}
