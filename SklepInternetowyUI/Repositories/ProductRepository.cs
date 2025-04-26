using Microsoft.EntityFrameworkCore;

namespace ProductSklepInternetowyUI.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product Product);
        Task DeleteProduct(Product Product);
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task UpdateProduct(Product Product);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product Product)
        {
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product Product)
        {
            _context.Products.Update(Product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product Product)
        {
            _context.Products.Remove(Product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetProductById(int id) => await _context.Products.FindAsync(id);

        public async Task<IEnumerable<Product>> GetProducts() => await _context.Products.Include(a=>a.Genre).ToListAsync();
    }
}
