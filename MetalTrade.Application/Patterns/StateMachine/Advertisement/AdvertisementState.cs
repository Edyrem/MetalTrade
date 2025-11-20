using MetalTrade.Application.Patterns.Factory.Advertisement;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.Application.Patterns.StateMachine.Advertisement.States;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Enums;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement
{
    public class AdvertisementState
    {
        private IAdvertisementState _state;
        public AdvertisementStatus Status { get; private set; }

        public AdvertisementState(string title, string description)
        {
            // по умолчанию Draft
            SetState(new DraftState(this));
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
