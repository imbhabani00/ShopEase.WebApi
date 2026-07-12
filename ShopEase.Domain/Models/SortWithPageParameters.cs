namespace ShopEase.Domain.Models
{
    public class SortWithPageParameters
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? SearchString { get; set; }
        public string? SortParameter { get; set; }
        public string? SortDirection { get; set; }
        public int? StatusId { get; set; }
    }
}