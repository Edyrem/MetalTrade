using MetalTrade.Application.Patterns.StateMachine.Advertisement;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.States;
using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.Factory.Advertisement
{
    public static class AdvertisementStateFactory
    {
        public static IAdvertisementState Create(AdvertisementState ad, AdvertisementStatus status) =>
            status switch
            {
                AdvertisementStatus.Draft => new DraftState(ad),
                AdvertisementStatus.Active => new ActiveState(ad),
                AdvertisementStatus.Archived => new ArchivedState(ad),
                AdvertisementStatus.Rejected => new RejectedState(ad),
                AdvertisementStatus.Deleted => new DeletedState(ad),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
    }
}
