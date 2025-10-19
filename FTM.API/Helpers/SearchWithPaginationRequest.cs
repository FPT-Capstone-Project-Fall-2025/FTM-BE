namespace FTM.API.Helpers
{
    public class SearchWithPaginationRequest
    {
        public string? Search { get; set; }
        public string? PropertyFilters { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? OrderBy { get; set; }
    }
}
