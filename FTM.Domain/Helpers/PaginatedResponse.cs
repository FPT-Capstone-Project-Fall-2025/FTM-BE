namespace FTM.Domain.Helpers
{
    /// <summary>
    /// Generic paginated response wrapper
    /// </summary>
    /// <typeparam name="T">The type of items in the response</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The list of items for the current page
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPrevious => Page > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNext => Page < TotalPages;

        /// <summary>
        /// Constructor for pagination
        /// </summary>
        public PaginatedResponse()
        {
        }

        /// <summary>
        /// Constructor with values
        /// </summary>
        public PaginatedResponse(List<T> items, int totalCount, int page, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
}
