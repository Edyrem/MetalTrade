using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        
        public async Task<AdvertisementImportResultDto> ImportFromExcelAsync(Stream excelStream, int userId)
        {
            var result = new AdvertisementImportResultDto();

            var products = await _productService.GetAllAsync();
            var productsDictionary = products.ToDictionary(
                p => p.Name.Trim().ToLower(),
                p => p.Id
            );

            using var workbook = new XLWorkbook(excelStream);
            var ws = workbook.Worksheet(1);
            
            if (ws == null)
            {
                result.Errors[0] = new List<string>
                {
                    "Excel файл не содержит листов"
                };
                return result;
            }

            var range = ws.RangeUsed();
            if (range == null)
            {
                result.Errors[0] = new List<string>
                {
                    "Excel файл пуст или не содержит данных"
                };
                return result;
            }
            var rows = range.RowsUsed();

            int rowNumber = 0;

            foreach (var row in rows)
            {
                rowNumber++;
                var errors = new List<string>();
                
                var priceText = row.Cell(3).GetString().Trim();
                if (!decimal.TryParse(priceText.Replace(',', '.'),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var price))
                {
                    errors.Add("Цена имеет неверный формат!");
                }
                
                var importDto = new AdvertisementImportDto
                {
                    Title = row.Cell(1).GetString(),
                    Body = row.Cell(2).GetString(),
                    Price = price,
                    City = row.Cell(4).GetString(),
                    PhoneNumber = row.Cell(5).GetString(),
                    ProductName = row.Cell(6).GetString(),
                    Address = row.Cell(7).GetString()
                };
                
                var validationErrors = Validate(importDto);
                errors.AddRange(validationErrors);
                
                var productKey = importDto.ProductName.Trim().ToLower();
                if (!productsDictionary.TryGetValue(productKey, out int productId))
                {
                    errors.Add($"Продукт '{importDto.ProductName}' не найден!");
                }
                
                if (errors.Any())
                {
                    result.Errors[rowNumber] = errors;
                    continue;
                }
                
                var advertisementDto = new AdvertisementDto
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
                result.CreatedCount++;
            }

            return result;
        }
        
        
        private List<string> Validate(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            
            return results.Select(r => r.ErrorMessage ?? "Ошибка валидации").ToList(); 
        }

    }
}
