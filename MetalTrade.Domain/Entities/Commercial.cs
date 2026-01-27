namespace MetalTrade.Domain.Entities;

public class Commercial
{
    public int Id { get; set; }

    public int AdvertisementId { get; set; }
    public Advertisement Advertisement { get; set; }

    public DateTime AdStartDate { get; set; }
    public DateTime AdEndDate { get; set; }

    public decimal Cost { get; set; } = 0;
}
