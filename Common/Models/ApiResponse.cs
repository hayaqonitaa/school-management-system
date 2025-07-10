namespace SchoolManagementSystem.Common.Models
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public PaginationMetadata? Pagination { get; set; }
    }

    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }

    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
