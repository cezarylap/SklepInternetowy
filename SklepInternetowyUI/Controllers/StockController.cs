using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductSklepInternetowyUI.Controllers
{
    [Authorize(Roles=nameof(Roles.Admin))] //Tylko admin
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepo;

        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public async Task<IActionResult> Index(string sTerm="") //Lista produktów ze stanem magazynowym, z możliwością wyszukiwania.
        {
            var stocks=await _stockRepo.GetStocks(sTerm);
            return View(stocks);
        }

        public async Task<IActionResult> ManangeStock(int ProductId) //Formularz zmiany stanu magazynowego.
        {
            var existingStock = await _stockRepo.GetStockByProductId(ProductId);
            var stock = new StockDTO
            {
                ProductId = ProductId,
                Quantity = existingStock != null
            ? existingStock.Quantity : 0
            };
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManangeStock(StockDTO stock) //Zapisuje zmiany stanu.
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stockRepo.ManageStock(stock);
                TempData["successMessage"] = "Stock is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
