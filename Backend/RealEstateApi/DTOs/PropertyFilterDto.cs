namespace RealEstateApi.DTOs;

public class PropertyFilterDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Year { get; set; }
    public string? OwnerId { get; set; }
    public string? CodeInternal { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PropertySearchResult
{
    public List<PropertyDto> Properties { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}