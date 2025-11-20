using MetalTrade.Application.Patterns.StateMachine.Advertisement.Abstractions;
using MetalTrade.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.States
{
    public class DeletedState : AdvertisementStateBase
    {
        public DeletedState(AdvertisementState ad) : base(ad)
        {
        }

        public override AdvertisementStatus Status => AdvertisementStatus.Deleted;
    }
}
