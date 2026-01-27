using AutoMapper;
using AutoMapper.QueryableExtensions;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Web.Controllers;

[Authorize(Roles = "admin,moderator")]
public class ProductController : Controller
{
     private readonly IProductService _productService;
     private readonly IMetalService _metalService;
     private readonly IMapper _mapper;

     public ProductController(
          IProductService productService, 
          IMetalService metalService,
          IMapper mapper)
     {
          _productService = productService;
          _metalService = metalService;
          _mapper = mapper;
     }

    public async Task<IActionResult> Index()
    {
        var filter = new ProductFilterDto
        {
            Name = Request.Query["name"],
            MetalTypeId = int.TryParse(Request.Query["metalTypeId"], out var mId) ? mId : null,
            Sort = Request.Query["sort"],
            Page = int.TryParse(Request.Query["page"], out var pg) ? pg : 1
        };

        List<ProductDto> productDtos = await _productService.GetFilteredAsync(filter);

        List<ProductViewModel> models = _mapper.Map<List<ProductViewModel>>(productDtos);

        ViewBag.Filter = filter;
        ViewBag.MetalTypes = await _metalService.GetAllAsync();
        return View(models);
    }

    public async Task<IActionResult> Create()
     {
          var metalTypes = await _metalService.GetAllAsync();

          ViewData["MetalTypes"] = new SelectList(metalTypes, "Id", "Name");
          return View();
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create(CreateProductViewModel model)
     {
          if (!ModelState.IsValid)
               return View(model);

          ProductDto productDto = _mapper.Map<ProductDto>(model);
          await _productService.CreateAsync(productDto);
          return RedirectToAction("Index");
     }

     

     public async Task<IActionResult> Details(int id)
     {
         ProductDto? productDto = await _productService.GetAsync(id);
         if (productDto == null)
             return RedirectToAction("Index");
         
         ProductViewModel model = _mapper.Map<ProductViewModel>(productDto);
         return View(model);
     }

     public async Task<IActionResult> Edit(int id)
     {
        ProductDto? productDto = await _productService.GetAsync(id);
        if (productDto == null)
            return RedirectToAction("Index");
         
        var metalTypes = await _metalService.GetAllAsync();
        EditProductViewModel model = _mapper.Map<EditProductViewModel>(productDto);
        ViewData["MetalTypes"] = new SelectList(metalTypes, "Id", "Name");

        return View(model);
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Edit(EditProductViewModel model)
     {
         if (!ModelState.IsValid)
             return View(model);
         
         ProductDto productDto =  _mapper.Map<ProductDto>(model);
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