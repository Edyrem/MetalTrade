using MetalTrade.Application.Patterns.StateMachine.Advertisement.Abstractions;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.States
{
    public class ActiveState : AdvertisementStateBase
    {
        public ActiveState(AdvertisementState ad) : base(ad)
        {
        }

        public override AdvertisementStatus Status => AdvertisementStatus.Active;

        public override IAdvertisementState MoveToArchived() => _ad.SetState(new ArchivedState(_ad));
        public override IAdvertisementState MoveToDeleted() => _ad.SetState(new DeletedState(_ad));
    }
}
