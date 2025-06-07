using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ProductSklepInternetowyUI.Repositories;

[Authorize(Roles = nameof(Roles.Admin))] //dostepne dla admina
public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;
    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TopNSoldProductModel>> GetTopNSellingProductsByDate(DateTime startDate, DateTime endDate) //Zwraca listê produktów, które sprzeda³y siê najlepiej w podanym przedziale dat.
    {
        var startDateParam = new SqlParameter("@startDate", startDate);
        var endDateParam = new SqlParameter("@endDate", endDate);
        var topFiveSoldProducts = await _context.Database.SqlQueryRaw<TopNSoldProductModel>("exec Usp_GetTopNSellingProductsByDate @startDate,@endDate", startDateParam, endDateParam).ToListAsync();
        return topFiveSoldProducts;
    }

}

public interface IReportRepository
{
    Task<IEnumerable<TopNSoldProductModel>> GetTopNSellingProductsByDate(DateTime startDate, DateTime endDate);
}