using ProductSklepInternetowyUI.Models;
using ProductSklepInternetowyUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;


namespace ProductSklepInternetowyUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm="",int genreId=0)
        {
            IEnumerable<Product> Products = await _homeRepository.GetProducts(sterm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            ProductDisplayModel ProductModel = new ProductDisplayModel
            {
              Products=Products,
              Genres=genres,
              STerm=sterm,
              GenreId=genreId
            };
            return View(ProductModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}