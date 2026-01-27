using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement.Abstractions
{
    public abstract class AdvertisementStateBase : IAdvertisementState
    {
        protected readonly AdvertisementState _ad;

        protected AdvertisementStateBase(AdvertisementState ad)
        {
            _ad = ad ?? throw new ArgumentNullException(nameof(ad));
        }

        public abstract AdvertisementStatus Status { get; }
        
        // поведение по умолчанию — запрещено
        public virtual IAdvertisementState MoveToActive()
        {
            if (Status == AdvertisementStatus.Active)
                return this;
            throw new InvalidOperationException($"{Status} -> Active: переход не разрешён.");
        }

        public virtual IAdvertisementState MoveToArchived()
        {
            if (Status == AdvertisementStatus.Archived)
                return this;
            throw new InvalidOperationException($"{Status} -> Archived: переход не разрешён.");
        }

        public virtual IAdvertisementState MoveToDeleted()
        {
            if (Status == AdvertisementStatus.Deleted)
                return this;
            throw new InvalidOperationException($"{Status} -> Deleted: переход не разрешён.");
        }

        public virtual IAdvertisementState MoveToRejected()
        {
            if (Status == AdvertisementStatus.Rejected)
                return this;
            throw new InvalidOperationException($"{Status} -> Rejected: переход не разрешён.");
        }
    }
}
