using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Helpers
{
    public class SearchWithPaginationRequest
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "propertyFilters")]
        public string? PropertyFilters { get; set; }

        [FromQuery(Name = "pageIndex")]
        public int PageIndex { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 5;

        [FromQuery(Name = "orderBy")]
        public string? OrderBy { get; set; }
    }
}
