using MetalTrade.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTrade.Business.Interfaces
{
    public interface IAdvertisementImportService
    {
        List<AdvertisementImportDto> ImportFromExcel(Stream excelStream);
    }
}
