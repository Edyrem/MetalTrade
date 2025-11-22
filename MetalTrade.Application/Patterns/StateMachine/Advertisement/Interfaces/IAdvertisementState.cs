using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces
{
    public interface IAdvertisementState
    {
        AdvertisementStatus Status { get; }

        IAdvertisementState MoveToActive();
        IAdvertisementState MoveToArchived();
        IAdvertisementState MoveToDeleted();
        IAdvertisementState MoveToRejected();
    }
}
