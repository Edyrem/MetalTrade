using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using MetalTrade.Web.Services.Advertisement;
using MetalTrade.Web.ViewModels.Advertisement;
using MetalTrade.Web.ViewModels.AdvertisementPhoto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Web.Controllers
{
    [Authorize]
    public class AdvertisementController : Controller
    {
        private readonly IAdvertisementService _adsService;
        private readonly UserManager<User> _userManager;
        private readonly AdvertisementPhotoSaveService _photoSaveService;
        private readonly MetalTradeDbContext _context;
        private readonly IMapper _mapper;

        public AdvertisementController(IAdvertisementService adsService, UserManager<User> userManager,
            IWebHostEnvironment env, MetalTradeDbContext context, IMapper mapper)
        {
            _adsService = adsService;
            _userManager = userManager;
            _photoSaveService = new AdvertisementPhotoSaveService(env);
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Create()
        {
            CreateViewModel model = new()
            {
                Products = [.. _context.Products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })]
            };
            return View(model);
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            User? user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid || user == null)
            {
                model.Products = [.. _context.Products.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Name
        })];
                return View(model);
            }

            // Используем маппер, но потом явно устанавливаем UserId
            var adsDto = _mapper.Map<AdvertisementDto>(model);
            adsDto.UserId = user.Id;

            if (model.Photoes != null && model.Photoes.Length > 0)
            {
                List<string> photoLinks = await _photoSaveService.SavePhotosAsync(model.Photoes);
                foreach (var link in photoLinks)
                {
                    adsDto.Photoes.Add(new AdvertisementPhotoDto { PhotoLink = link });
                }
            }

            await _adsService.CreateAsync(adsDto);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
             var adsDtos = await _adsService.GetAllAsync();
            var models = _mapper.Map<List<AdvertisementViewModel>>(adsDtos);
            return View(models);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var adsDto = await _adsService.GetAsync(id);
            if (adsDto == null) return RedirectToAction("Index");

            var model = _mapper.Map<AdvertisementViewModel>(adsDto);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            List<string> ExistingPhotos;
            var adsDto = await _adsService.GetAsync(id);
            if (adsDto == null) return RedirectToAction("Index");
            var model = _mapper.Map<EditViewModel>(adsDto);

            model.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = p.Id == model.ProductId
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Products = _context.Products
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name, Selected = p.Id == model.ProductId })
                    .ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден");
                return View(model);
            }

            var adsDto = _mapper.Map<AdvertisementDto>(model);

            var existingAds = await _adsService.GetAsync(model.Id);
            if (existingAds != null)
            {
                adsDto.UserId = existingAds.UserId; 
            }

            await _adsService.UpdateAsync(adsDto);
            return RedirectToAction("Details", new { id = model.Id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var adsDto = await _adsService.GetAsync(id);
            if (adsDto == null) 
                return RedirectToAction("Index");

            var model = _mapper.Map<DeleteViewModel>(adsDto);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            await _adsService.DeleteAsync(model.Id);
            return RedirectToAction("Index");
        }
    }
}
