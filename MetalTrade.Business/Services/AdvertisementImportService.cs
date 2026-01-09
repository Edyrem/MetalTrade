using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MetalTrade.Business.Services
{
    public class AdvertisementImportService : IAdvertisementImportService
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly IProductService _productService;

        public AdvertisementImportService(IAdvertisementService advertisementService, IProductService productService)
        {
            _advertisementService = advertisementService;
            _productService = productService;
        }
        
        public async Task<int> ImportFromExcelAsync(Stream excelStream, int userId)
        {
            int createdCount = 0;
            var products = await _productService.GetAllAsync();
            var productsDictionary = products.ToDictionary(
                p  => p.Name.Trim().ToLower(),
                p=>p.Id);
            
            using var workbook = new XLWorkbook(excelStream);
            var ws = workbook.Worksheet(1);
            var rows = ws.RangeUsed().RowsUsed().Skip(1);
            
            foreach (var row in rows)
            {
                var importDto = new AdvertisementImportDto
                {
                    Title = row.Cell(1).GetString(),
                    Body = row.Cell(2).GetString(),
                    Price = row.Cell(3).GetValue<decimal>(),
                    City = row.Cell(4).GetString(),
                    PhoneNumber = row.Cell(5).GetString(),
                    ProductName = row.Cell(6).GetString(),
                    Address = row.Cell(7).GetString()
                };

                if (!IsValid(importDto))
                    continue;

                var importDtoProduct = importDto.ProductName.Trim().ToLower();
                if (!productsDictionary.TryGetValue(importDtoProduct, out int productId))
                    continue;

                var advertisementDto = new AdvertisementDto()
                {
                    Title = importDto.Title,
                    Body = importDto.Body,
                    Price = importDto.Price,
                    City = importDto.City,
                    PhoneNumber = importDto.PhoneNumber,
                    ProductId = productId,
                    Address = importDto.Address,
                    UserId = userId
                };
                await _advertisementService.CreateAsync(advertisementDto);
                createdCount++;
            }

            return createdCount;
        }
        
        private bool IsValid(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }
    }
}
