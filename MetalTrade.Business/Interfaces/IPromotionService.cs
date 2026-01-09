using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionService
    {
        Task UpdatePromotionAsync(int advertisementId);
        Task DeactivatePromotionAsync(int advertisementId);
        Task UpdateUserPromotionAsync(int userId);
        Task DeactivateUserPromotionAsync(int userId);
    }
}
