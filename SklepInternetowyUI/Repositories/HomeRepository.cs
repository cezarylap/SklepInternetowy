

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
            // this code is demonstrated in the video tutorial.
            // It contains very bad practice.
            // 1st bad practice is in the where condtion
            // 2nd one is: filter by genre. I am loading all the data into memory then filtering it. Performance will decrease with data size.
            // I have commented out that code and rewrite it. 

            //sTerm = sTerm.ToLower();
            //IEnumerable<Product> Products = await (from Product in _db.Products
            //             join genre in _db.Genres
            //             on Product.GenreId equals genre.Id
            //             join stock in _db.Stocks
            //             on Product.Id equals stock.ProductId
            //             into Product_stocks
            //             from ProductWithStock in Product_stocks.DefaultIfEmpty()
            //             where string.IsNullOrWhiteSpace(sTerm) || (Product != null && Product.ProductName.ToLower().StartsWith(sTerm))
            //             select new Product
            //             {
            //                 Id = Product.Id,
            //                 Image = Product.Image,
            //                 AuthorName = Product.AuthorName,
            //                 ProductName = Product.ProductName,
            //                 GenreId = Product.GenreId,
            //                 Price = Product.Price,
            //                 GenreName = genre.GenreName,
            //                 Quantity=ProductWithStock==null? 0:ProductWithStock.Quantity
            //             }
            //             ).ToListAsync();


            //if (genreId > 0)
            //{

            //    Products = Products.Where(a => a.GenreId == genreId).ToList();
            //}

            // refactored code
            // In this code we are first building query, then rebuilding that query on the basis of filter. Query is translated into sql when we call .ToListAsync() method.

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
