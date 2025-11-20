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
            LoadStateFromEnum();
        }

        public void SetState(IAdvertisementState state)
        {
            _state = state;
            Status = state.Status;
        }

        // восстановление состояния после загрузки из БД
        public void LoadStateFromEnum()
        {
            _state = AdvertisementStateFactory.Create(this, Status);
        }

        public void MoveToActive() => _state.MoveToActive();
        public void MoveToArchived() => _state.MoveToArchived();
        public void MoveToDeleted() => _state.MoveToDeleted();
        public void MoveToRejected() => _state.MoveToRejected();
    }
}
