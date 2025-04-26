using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProductSklepInternetowyUI.Data;

public class DbSeeder
{
    public static async Task SeedDefaultData(IServiceProvider service)
    {
        try
        {
            var context = service.GetService<ApplicationDbContext>();

            // this block will check if there are any pending migrations and apply them
            if ((await context.Database.GetPendingMigrationsAsync()).Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            // create admin role if not exists
            var adminRoleExists = await roleMgr.RoleExistsAsync(Roles.Admin.ToString());

            if (!adminRoleExists)
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }

            // create user role if not exists
            var userRoleExists = await roleMgr.RoleExistsAsync(Roles.User.ToString());

            if (!userRoleExists)
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }

            // create admin user
            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userInDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }

            if (!context.Genres.Any())
            {
                await SeedGenreAsync(context);
            }

            if (!context.Products.Any())
            {
                await SeedProductsAsync(context);
                // update stock table
                await context.Database.ExecuteSqlRawAsync(@"
                     INSERT INTO Stock(ProductId,Quantity) 
                     SELECT 
                     b.Id,
                     10 
                     FROM Product b
                     WHERE NOT EXISTS (
                     SELECT * FROM [Stock]
                     );
           ");
            }

            if (!context.orderStatuses.Any())
            {
                await SeedOrderStatusAsync(context);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    #region private methods

    private static async Task SeedGenreAsync(ApplicationDbContext context)
    {
        var genres = new[]
         {
            new Genre { GenreName = "Konsole" },
            new Genre { GenreName = "Komputery" },
            new Genre { GenreName = "Foto i kamery" },
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }

    private static async Task SeedOrderStatusAsync(ApplicationDbContext context)
    {
        var orderStatuses = new[]
        {
            new OrderStatus { StatusId = 1, StatusName = "W realizacji" },
            new OrderStatus { StatusId = 2, StatusName = "Wysłane" },
            new OrderStatus { StatusId = 3, StatusName = "Dostarczone" },
            new OrderStatus { StatusId = 4, StatusName = "Anulowane" },
            new OrderStatus { StatusId = 5, StatusName = "Zwrócone" },
        };

        await context.orderStatuses.AddRangeAsync(orderStatuses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        var Products = new List<Product>
        {
            new Product { ProductName = "Sony PlayStation 5", AuthorName = "Sony", Price = 2100, GenreId = 1 },
            new Product { ProductName = "Sony PlayStation 5 DualSense", AuthorName = "Sony", Price = 319, GenreId = 1 },
            new Product { ProductName = "Monitor Predator XB273UV3bmiiprzx 27", AuthorName = "Sony", Price = 1449, GenreId = 2 },
            new Product { ProductName = "Dysk Samsung 980 500GB M2", AuthorName = "Samsung", Price = 199, GenreId = 2 },
            new Product { ProductName = "Pamięć RAM GOODRAM IRDM 32GB 6000MHz", AuthorName = "GOODRAM", Price = 417, GenreId = 2 },
            new Product { ProductName = "Procesor INTEL Core Ultra 9-285K", AuthorName = "Intel", Price = 2939, GenreId = 2 },
            new Product { ProductName = "Aparat bezlusterkowy SONY Alpha A7 III", AuthorName = "Sony", Price = 6489, GenreId = 3 },
            new Product { ProductName = "Aparat bezlusterkowy CANON EOS R8", AuthorName = "Canon", Price = 5849, GenreId = 3 },
            new Product { ProductName = "Gimbal DJI Osmo Mobile 7P Czarny", AuthorName = "DJI", Price = 698, GenreId = 3 },
        };
        await context.Products.AddRangeAsync(Products);
        await context.SaveChangesAsync();
    }
    #endregion
}
