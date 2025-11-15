namespace MetalTrade.DataAccess.Interfaces.Repositories;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}