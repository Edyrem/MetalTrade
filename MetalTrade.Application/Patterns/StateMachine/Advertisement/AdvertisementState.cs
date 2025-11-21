using MetalTrade.Application.Patterns.Factory.Advertisement;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.States;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement
{
    public class AdvertisementState
    {
        public AdvertisementStatus Status { get; private set; }
        
        private IAdvertisementState _state = null!;

        public AdvertisementState()
        {
            // по умолчанию Draft
            SetState(new DraftState(this));
        }

        public AdvertisementState(AdvertisementStatus status)
        {
            Status = status;
            _state = LoadStateFromEnum();
        }

        public IAdvertisementState SetState(IAdvertisementState state)
        {
            _state = state;
            Status = state.Status;
            return _state;
        }

        // восстановление состояния после загрузки из БД
        public IAdvertisementState LoadStateFromEnum()
        {
            return AdvertisementStateFactory.Create(this, Status);
        }

        public IAdvertisementState MoveToActive() => _state.MoveToActive();
        public IAdvertisementState MoveToArchived() => _state.MoveToArchived();
        public IAdvertisementState MoveToDeleted() => _state.MoveToDeleted();
        public IAdvertisementState MoveToRejected() => _state.MoveToRejected();
    }
}
