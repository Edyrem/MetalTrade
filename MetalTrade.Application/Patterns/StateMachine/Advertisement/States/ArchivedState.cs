using MetalTrade.Application.Patterns.StateMachine.Advertisement.Abstractions;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.States
{
    public class ArchivedState : AdvertisementStateBase
    {
        public ArchivedState(AdvertisementState ad) : base(ad)
        {
        }

        public override AdvertisementStatus Status => AdvertisementStatus.Archived;

        public override IAdvertisementState MoveToActive() => _ad.SetState(new ActiveState(_ad));
        public override IAdvertisementState MoveToDeleted() => _ad.SetState(new DeletedState(_ad));
    }
}
