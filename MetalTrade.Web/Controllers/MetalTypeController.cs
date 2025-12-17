using AutoMapper;
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
    private readonly IMapper _mapper;

    public MetalTypeController(IMetalService metalService, IMapper mapper)
    {
        _metalService = metalService;
        _mapper = mapper;
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

         MetalTypeDto metalDto = _mapper.Map<MetalTypeDto>(model);
         await _metalService.CreateAsync(metalDto);
         return RedirectToAction("Index");
     }

     public async Task<IActionResult> Index()
     {
         List<MetalTypeDto> metalDtos = await _metalService.GetAllAsync();
         
         List<MetalTypeViewModel> models = _mapper.Map<List<MetalTypeViewModel>>(metalDtos);
         
         return View(models);
     }

     public async Task<IActionResult> Details(int id)
     {
         MetalTypeDto? metalDto = await _metalService.GetAsync(id);
         if (metalDto == null)
             return RedirectToAction("Index");
         
         MetalTypeViewModel model = _mapper.Map<MetalTypeViewModel>(metalDto);
         return View(model);
     }

     public async Task<IActionResult> Edit(int id)
     {
         MetalTypeDto? metalDto = await _metalService.GetAsync(id);
         if (metalDto == null)
             return RedirectToAction("Index");
         
         EditMetalViewModel model = _mapper.Map<EditMetalViewModel>(metalDto);
         return View(model);
     }

     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> Edit(EditMetalViewModel model)
     {
         if (!ModelState.IsValid)
             return View(model);
         
         MetalTypeDto metalDto = _mapper.Map<MetalTypeDto>(model);
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