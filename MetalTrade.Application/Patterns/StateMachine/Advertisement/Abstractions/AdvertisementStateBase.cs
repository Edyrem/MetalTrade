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
        public virtual void MoveToActive()
        {
            if (Status == AdvertisementStatus.Active)
                return;
            throw new InvalidOperationException($"{Status} -> Active: переход не разрешён.");
        }

        public virtual void MoveToArchived()
        {
            if (Status == AdvertisementStatus.Archived)
                return;
            throw new InvalidOperationException($"{Status} -> Archived: переход не разрешён.");
        }

        public virtual void MoveToDeleted()
        {
            if (Status == AdvertisementStatus.Deleted)
                return;
            throw new InvalidOperationException($"{Status} -> Deleted: переход не разрешён.");
        }

        public virtual void MoveToRejected()
        {
            if (Status == AdvertisementStatus.Rejected)
                return;
            throw new InvalidOperationException($"{Status} -> Rejected: переход не разрешён.");
        }
    }
}
