using MetalTrade.Business.Services;
using MetalTrade.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace MetalTrade.Web.Controllers
{
    public class AdvertisementController : Controller
    {
        AdvertisementService adsService;
        public AdvertisementController(MetalTradeDbContext context)
        {
            adsService = new AdvertisementService(context);
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
