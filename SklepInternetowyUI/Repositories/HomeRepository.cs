

using Microsoft.EntityFrameworkCore;

namespace ProductSklepInternetowyUI.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProducts(string sTerm = "", int genreId = 0)
        {
            var ProductQuery = _db.Products
               .AsNoTracking()
               .Include(x => x.Genre)
               .Include(x => x.Stock)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                ProductQuery = ProductQuery.Where(b => b.ProductName.StartsWith(sTerm.ToLower()));
            }

            if (genreId > 0)
            {
                ProductQuery = ProductQuery.Where(b => b.GenreId == genreId);
            }

            var Products = await ProductQuery
                .AsNoTracking()
                .Select(Product => new Product
                {
                    Id = Product.Id,
                    Image = Product.Image,
                    AuthorName = Product.AuthorName,
                    ProductName = Product.ProductName,
                    GenreId = Product.GenreId,
                    Price = Product.Price,
                    GenreName = Product.Genre.GenreName,
                    Quantity = Product.Stock == null ? 0 : Product.Stock.Quantity
                }).ToListAsync();
            return Products;

        }
    }
}
