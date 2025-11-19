using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetalTrade.Application.Patterns.Factory.Advertisement
{
    public static class AdvertisementStateFactory
    {
        public static IAdvertisementState Create(AdvertisementDto ad, AdvertisementStatus status) =>
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
