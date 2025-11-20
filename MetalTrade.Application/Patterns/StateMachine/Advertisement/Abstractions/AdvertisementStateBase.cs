using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Business.Dtos;
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
            => throw new InvalidOperationException($"{Status} -> Active: переход не разрешён.");

        public virtual void MoveToArchived()
            => throw new InvalidOperationException($"{Status} -> Archived: переход не разрешён.");

        public virtual void MoveToDeleted()
            => throw new InvalidOperationException($"{Status} -> Deleted: переход не разрешён.");

        public virtual void MoveToRejected()
            => throw new InvalidOperationException($"{Status} -> Rejected: переход не разрешён.");
    }
}
