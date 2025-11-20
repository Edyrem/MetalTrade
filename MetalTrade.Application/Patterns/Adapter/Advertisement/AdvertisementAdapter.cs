using MetalTrade.Application.Patterns.StateMachine.Advertisement;
using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Application.Patterns.Adapter.Advertisement
{
    public class AdvertisementAdapter
    {
        private IAdvertisementRepository _repository;
        private int _advertisementId;
        private AdvertisementState _advertisementState = null!;

        public AdvertisementAdapter(IAdvertisementRepository repository, int advertisementId)
        {
            _repository = repository;
            _advertisementId = advertisementId;

            var status = _repository.GetStatusAsync(_advertisementId).Result;
            _advertisementState = new AdvertisementState(status);
        }

        public async Task MoveToActiveAsync()
        {
            _advertisementState.MoveToActive();
            await _repository.SetStatusAsync(_advertisementId, _advertisementState.Status);
        }

        public async Task MoveToArchivedAsync()
        {
            _advertisementState.MoveToArchived();
            await _repository.SetStatusAsync(_advertisementId, _advertisementState.Status);
        }

        public async Task MoveToRejectedAsync()
        {
            _advertisementState.MoveToRejected();
            await _repository.SetStatusAsync(_advertisementId, _advertisementState.Status);
        }

        public async Task MoveToDeletedAsync()
        {
            _advertisementState.MoveToDeleted();
            await _repository.SetStatusAsync(_advertisementId, _advertisementState.Status);
        }
    }
}
