using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces
{
    public interface IAdvertisementState
    {
        AdvertisementStatus Status { get; }

        void MoveToActive();
        void MoveToArchived();
        void MoveToDeleted();
        void MoveToRejected();
    }
}
