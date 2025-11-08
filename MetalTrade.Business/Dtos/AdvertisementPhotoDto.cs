using MetalTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Dtos
{
    internal class AdvertisementPhotoDto
    {
        public int Id { get; set; }
        public string PhotoLink { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisement? Advertisement { get; set; }
    }
}
