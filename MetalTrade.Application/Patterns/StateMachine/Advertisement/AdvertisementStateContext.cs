using MetalTrade.Application.Patterns.StateMachine.Advertisement.Interfaces;
using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Application.Patterns.StateMachine.Advertisement
{
    public class AdvertisementStateContext
    {
        private IAdvertisementRepository _repository;

        public AdvertisementStateContext(IAdvertisementRepository repository)
        {
            _repository = repository;          
        }

        private async Task MoveAsync(
            int advertisementId,
            Func<AdvertisementState, IAdvertisementState> transition)
        {
            var status = await _repository.GetStatusAsync(advertisementId);
            var state = new AdvertisementState(status);
            transition(state);
            await _repository.SetStatusAsync(advertisementId, state.Status);
        }

        public async Task MoveToActiveAsync(int advertisementId)
        {
            await MoveAsync(advertisementId, s => s.MoveToActive());
        }

        public async Task MoveToArchivedAsync(int advertisementId)
        {
            await MoveAsync(advertisementId, s => s.MoveToArchived());
        }

        public async Task MoveToRejectedAsync(int advertisementId)
        {
            await MoveAsync(advertisementId, s => s.MoveToRejected());
        }

        public async Task MoveToDeletedAsync(int advertisementId)
        {
            await MoveAsync(advertisementId, s => s.MoveToDeleted());
        }
    }
}
