using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Dtos
{
    public class PromotionStatusResult
    {
        public bool ShouldBeActive { get; set; }
        public bool HasChanged { get; set; }
        public string? Reason { get; set; }
    }
}
