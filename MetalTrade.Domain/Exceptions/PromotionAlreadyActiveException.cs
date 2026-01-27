
namespace MetalTrade.Domain.Exceptions
{
    public class PromotionAlreadyActiveException: Exception
    {
        public int EntityId { get; }
        public string PromotionType { get; }

        public PromotionAlreadyActiveException(int entityId, string promotionType): base($"A promotion of type '{promotionType}' is already active for entity with ID {entityId}.")
        {
            EntityId = entityId;
            PromotionType = promotionType;
        }
    }
}
