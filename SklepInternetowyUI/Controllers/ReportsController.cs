using Microsoft.AspNetCore.Mvc;

namespace ProductSklepInternetowyUI.Controllers;
public class ReportsController : Controller
{
    private readonly IReportRepository _reportRepository;
    public ReportsController(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }
    public async Task<ActionResult> TopFiveSellingProducts(DateTime? sDate = null, DateTime? eDate = null) //Zwraca dane na temat najcz�ciej kupowanych produkt�w w wybranym przedziale dat.
    {
        try
        {
            DateTime startDate = sDate ?? DateTime.UtcNow.AddDays(-7);   // Z ostatnich 7 dni
            DateTime endDate = eDate ?? DateTime.UtcNow;
            var topFiveSellingProducts = await _reportRepository.GetTopNSellingProductsByDate(startDate, endDate);
            var vm = new TopNSoldProductsVm(startDate, endDate, topFiveSellingProducts);
            return View(vm);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Something went wrong";
            return RedirectToAction("Index", "Home");
        }
    }
}