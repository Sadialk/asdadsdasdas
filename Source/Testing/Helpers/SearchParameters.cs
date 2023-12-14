namespace Testing.Helpers
{
    public class SearchParameters
    {
        private int _pageSize = 100;
        private int _pageNumber = 1;
        private const int MaxPageSize = 200;
        public int? PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value ?? _pageNumber;
        }
        public int? PageSize
        {
            get => _pageSize> MaxPageSize ? MaxPageSize : _pageSize;
            set => _pageSize = value ?? _pageSize;
        }
    }
}
