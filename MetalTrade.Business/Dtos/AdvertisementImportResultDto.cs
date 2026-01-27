namespace MetalTrade.Business.Dtos;

public class AdvertisementImportResultDto
{
    public int CreatedCount { get; set; }
    public Dictionary<int, List<string>> Errors { get; set; } = new();
}