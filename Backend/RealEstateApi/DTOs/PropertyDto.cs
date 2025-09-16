namespace RealEstateApi.DTOs;

public class PropertyDto
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CodeInternal { get; set; } = string.Empty;
    public int Year { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    
    // Propiedades que usa tu PropertyService
    public string IdOwner { get; set; } = string.Empty;  // Alias para OwnerId
    public string Image { get; set; } = string.Empty;    // Para imagen principal
}

public class CreatePropertyDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CodeInternal { get; set; } = string.Empty;
    public int Year { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}

public class UpdatePropertyDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? Price { get; set; }
    public string? CodeInternal { get; set; }
    public int? Year { get; set; }
    public string? OwnerId { get; set; }
}