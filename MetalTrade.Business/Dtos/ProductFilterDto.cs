using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Dtos
{
    public class ProductFilterDto
    {
        public string? Name { get; set; }
        public int? MetalTypeId { get; set; }
        public string? Sort { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
