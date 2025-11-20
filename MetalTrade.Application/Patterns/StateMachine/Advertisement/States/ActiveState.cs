using MetalTrade.Application.Patterns.StateMachine.Advertisement.Abstractions;
using MetalTrade.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.States
{
    public class ActiveState : AdvertisementStateBase
    {
        public ActiveState(AdvertisementState ad) : base(ad)
        {
        }

        public override AdvertisementStatus Status => AdvertisementStatus.Active;

        public override void MoveToArchived() => _ad.SetState(new ArchivedState(_ad));
        public override void MoveToDeleted() => _ad.SetState(new DeletedState(_ad));
    }
}
