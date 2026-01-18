using MetalTrade.Domain.Abstraction;
using MetalTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface IPromotionRepository<T>: IRepository<T> where T : TimedPromotion
    {
        Task AddAsync(T promotion);
        Task<T?> GetActiveAsync();
        Task<IEnumerable<T>> GetAllActiveAsync();
        Task<bool> HasActiveAsync();
    }
}
