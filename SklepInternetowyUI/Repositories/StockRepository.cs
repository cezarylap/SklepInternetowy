using Microsoft.EntityFrameworkCore;

namespace ProductSklepInternetowyUI.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockByProductId(int ProductId) => await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == ProductId);

        public async Task ManageStock(StockDTO stockToManage)
        {
            // jeżeli w magazynie niema produktu wtedy go dodaj, jeżeli w magazynie jest produkt to zmień ilość
            var existingStock = await GetStockByProductId(stockToManage.ProductId);
            if (existingStock is null)
            {
                var stock = new Stock { ProductId = stockToManage.ProductId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from Product in _context.Products
                                join stock in _context.Stocks
                                on Product.Id equals stock.ProductId
                                into Product_stock
                                from ProductStock in Product_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || Product.ProductName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    ProductId = Product.Id,
                                    ProductName = Product.ProductName,
                                    Quantity = ProductStock == null ? 0 : ProductStock.Quantity
                                }
                                ).ToListAsync();
            return stocks;
        }

    }

    public interface IStockRepository
    {
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
        Task<Stock?> GetStockByProductId(int ProductId);
        Task ManageStock(StockDTO stockToManage);
    }
}
