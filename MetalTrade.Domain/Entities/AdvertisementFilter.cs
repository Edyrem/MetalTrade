namespace MetalTrade.Domain.Entities;

public class AdvertisementFilter
{
    public string? Title { get; set; }
    public string? City { get; set; }
    public int? MetalTypeId { get; set; }
    
    public int? ProductId { get; set; }

    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    
    public string? Sort { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 21;

    public DateTime? DateFromUtc => DateFrom?.ToUniversalTime();

    public DateTime? DateToUtc => DateTo?.ToUniversalTime();
    
}


