using MetalTrade.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces
{
    public interface IPromotionValidator
    {
        Task ValidateCanActivateasync<T>(int entityId) where T : TimedPromotion;
    }
}
