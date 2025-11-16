using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.Web.ViewModels.MetalType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers;

[Authorize(Roles = "admin, moderator")]
public class MetalTypeController : Controller
{
     private readonly IMetalService _metalService;

     public MetalTypeController(IMetalService metalService)
     {
         _metalService = metalService;
     }
     
     public IActionResult Create()
     {
         return View();
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create(CreateMetalViewModel model)
     {
         if (!ModelState.IsValid)
             return View(model);

         MetalTypeDto metalDto = new()
         {
             Name = model.Name
         };
         await _metalService.CreateAsync(metalDto);
         return RedirectToAction("Index");
     }

     public async Task<IActionResult> Index()
     {
         List<MetalTypeDto> metalDtos = await _metalService.GetAllAsync();
         
         List<MetalTypeViewModel> models = metalDtos.Select(metalType => new MetalTypeViewModel
         {
             Id = metalType.Id,
             Name = metalType.Name
         }).ToList();
         
         return View(models);
     }

     public async Task<IActionResult> Details(int id)
     {
         MetalTypeDto? metalDto = await _metalService.GetAsync(id);
         if (metalDto == null)
             return RedirectToAction("Index");
         
         MetalTypeViewModel model = new()
         {
             Id = metalDto.Id,
             Name = metalDto.Name
         };
         return View(model);
     }

     public async Task<IActionResult> Edit(int id)
     {
         MetalTypeDto? metalDto = await _metalService.GetAsync(id);
         if (metalDto == null)
             return RedirectToAction("Index");
         
         EditMetalViewModel model = new()
         {
             Id = metalDto.Id,
             Name = metalDto.Name
         };
         return View(model);
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Edit(EditMetalViewModel model)
     {
         if (!ModelState.IsValid)
             return View(model);
         
         MetalTypeDto metalDto = new()
         {
             Id = model.Id,
             Name = model.Name
         };
         await _metalService.UpdateAsync(metalDto);
         return RedirectToAction("Index");
     }

     public async Task<IActionResult> Delete(int id)
     {
         MetalTypeDto? metalDto = await _metalService.GetAsync(id);
         if (metalDto != null)
             return View(new DeleteMetalViewModel { Id = metalDto.Id, Name = metalDto.Name });
         return RedirectToAction("Index");
     }

     [HttpPost]
     public async Task<IActionResult> Delete(DeleteMetalViewModel model)
     {
         await _metalService.DeleteAsync(model.Id);
         return RedirectToAction("Index");
     }
}