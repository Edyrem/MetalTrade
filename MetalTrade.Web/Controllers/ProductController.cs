using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.ViewModels.MetalType;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MetalTrade.Web.Controllers;

[Authorize(Roles = "admin, moderator")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly IMetalService _metalService;

     public ProductController(IProductService productService, IMetalService metalService)
     {
         _productService = productService;
         _metalService = metalService;
     }
     
     public async Task<IActionResult> Create()
     {
         var metalTypes = await _metalService.GetAllAsync();
         
         CreateProductViewModel model = new()
         {
             MetalTypes = metalTypes.Select(m => new SelectListItem
             {
                 Value = m.Id.ToString(),
                 Text = m.Name
             }).ToList()
         };
         
         return View(model);
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create(CreateProductViewModel model)
     {
         if (!ModelState.IsValid)
             return View(model);

         ProductDto productDto = new()
         {
             Name = model.Name,
             MetalTypeId = model.MetalTypeId
         };
         await _productService.CreateAsync(productDto);
         return RedirectToAction("Index");
     }

     public async Task<IActionResult> Index()
     {
         List<ProductDto> productDtos = await _productService.GetAllAsync();
         
         List<ProductViewModel> models = productDtos.Select(product => new ProductViewModel()
         {
             Id = product.Id,
             Name = product.Name,
             MetalTypeId = product.MetalTypeId,
             MetalType = new MetalTypeViewModel
                 {
                     Id = product.MetalType.Id,
                     Name = product.MetalType.Name
                 }
         }).ToList();
         
         return View(models);
     }

     public async Task<IActionResult> Details(int id)
     {
         ProductDto? productDto = await _productService.GetAsync(id);
         if (productDto == null)
             return RedirectToAction("Index");
         
         ProductViewModel model = new()
         {
             Id = productDto.Id,
             Name = productDto.Name,
             MetalTypeId = productDto.MetalTypeId,
             MetalType = new MetalTypeViewModel
                 {
                     Id = productDto.MetalType.Id,
                     Name = productDto.MetalType.Name
                 }
         };
         return View(model);
     }

     public async Task<IActionResult> Edit(int id)
     {
         ProductDto? productDto = await _productService.GetAsync(id);
         if (productDto == null)
             return RedirectToAction("Index");
         
         var metalTypes = await _metalService.GetAllAsync();
         EditProductViewModel model = new()
         {
             Id = productDto.Id,
             Name = productDto.Name,
             MetalTypeId = productDto.MetalTypeId
             
         };
         model.MetalTypes = metalTypes.Select(m => new SelectListItem
         {
             Value = m.Id.ToString(),
             Text = m.Name,
             Selected = m.Id == model.MetalTypeId
         }).ToList();
         return View(model);
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Edit(EditProductViewModel model)
     {
         if (!ModelState.IsValid)
             return View(model);
         
         ProductDto productDto = new()
         {
             Id = model.Id,
             Name = model.Name,
             MetalTypeId = model.MetalTypeId
         };
         await _productService.UpdateAsync(productDto);
         return RedirectToAction("Index");
     }

     public async Task<IActionResult> Delete(int id)
     {
         ProductDto? productDto = await _productService.GetAsync(id);
         if (productDto != null)
             return View(new DeleteProductViewModel() { Id = productDto.Id, Name = productDto.Name, MetalTypeId = productDto.MetalTypeId});
         return RedirectToAction("Index");
     }

     [HttpPost]
     public async Task<IActionResult> Delete(DeleteProductViewModel model)
     {
         await _productService.DeleteAsync(model.Id);
         return RedirectToAction("Index");
     }
}