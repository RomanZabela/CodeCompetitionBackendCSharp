namespace CodeCompetitionBackendCSharp.RequestFeatures
{
    public class TaskCategoryParameters
    {
        public bool switchOff { get; set; } = false;

        public string switchOffString { get; set; } = "0";

        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public int _pageSize = 2;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}
