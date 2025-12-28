using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MetalTrade.Business.Services
{
    public class AdvertisementImportService : IAdvertisementImportService
    {
        public List<AdvertisementImportDto> ImportFromExcel(Stream excelStream)
        {
            var result = new List<AdvertisementImportDto>();

            using var workbook = new XLWorkbook(excelStream);
            var ws = workbook.Worksheet(1);

            var rows = ws.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                result.Add(new AdvertisementImportDto
                {
                    Title = row.Cell(1).GetString(),
                    Price = row.Cell(2).GetValue<decimal>(),
                    Unit = row.Cell(3).GetString(),
                    Description = row.Cell(4).GetString(),
                    City = row.Cell(5).GetString(),
                    Contacts = row.Cell(6).GetString()
                });
            }

            return result;
        }
    }
}
