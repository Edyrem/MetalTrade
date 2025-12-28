using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Dtos
{
    public class AdvertisementImportDto
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Contacts { get; set; }
        public int? MetalTypeId { get; set; }
        public int? ProductId { get; set; }
    }
}
