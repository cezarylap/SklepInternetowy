using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ProductSklepInternetowyUI.Data;
using ProductSklepInternetowyUI.Models;
using ProductSklepInternetowyUI.Repositories;

namespace SklepInternetowy.Tests.IntegrationTests
{
    public class ProductRepositoryIntegrationTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            // Każdy test dostaje nową, czystą bazę InMemory
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AddProduct_ThenRetrieve_ShouldContainNewProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            await using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            // Dodanie gatunku, ponieważ GetProducts używa Include(a => a.Genre)
            var genre = new Genre { GenreName = "TestGenre" };
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var product = new Product
            {
                ProductName = "TestProduct",
                Price = 99.99,
                AuthorName = "Integration Tester",
                GenreId = genre.Id
            };

            // Act
            await repo.AddProduct(product);
            var products = (await repo.GetProducts()).ToList();

            // Assert
            Assert.NotEmpty(products);
            Assert.Contains(products, p =>
                p.ProductName == "TestProduct" &&
                Math.Abs(p.Price - 99.99) < 0.01 &&
                p.Genre != null &&
                p.Genre.GenreName == "TestGenre"
            );
        }

        [Fact]
        public async Task GetProductById_ShouldReturnCorrectProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            await using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            var genre = new Genre { GenreName = "GenreForByIdTest" };
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var product = new Product
            {
                ProductName = "ByIdProduct",
                Price = 55.5,
                AuthorName = "Integration Tester",
                GenreId = genre.Id
            };
            await repo.AddProduct(product);

            // Act
            var retrieved = await repo.GetProductById(product.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("ByIdProduct", retrieved.ProductName);
            Assert.Equal(55.5, retrieved.Price);
            Assert.Equal(genre.Id, retrieved.GenreId);
        }

        [Fact]
        public async Task DeleteProduct_ShouldRemoveProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            await using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            var product = new Product
            {
                ProductName = "DeleteMe",
                Price = 10.0,
                AuthorName = "Tester",
                Genre = new Genre { GenreName = "SampleGenre" }
            };

            await repo.AddProduct(product);

            // Act
            await repo.DeleteProduct(product);
            var products = (await repo.GetProducts()).ToList();

            // Assert
            Assert.DoesNotContain(products, p => p.ProductName == "DeleteMe");
        }
        [Fact]
        public async Task UpdateProduct_ShouldModifyProductData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            await using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            var genre = new Genre { GenreName = "UpdateGenre" };
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var product = new Product
            {
                ProductName = "OriginalName",
                Price = 20.0,
                AuthorName = "Original Author",
                GenreId = genre.Id
            };

            await repo.AddProduct(product);

            // Act
            product.ProductName = "UpdatedName";
            product.Price = 99.99;
            await repo.UpdateProduct(product);

            var updated = await repo.GetProductById(product.Id);

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("UpdatedName", updated.ProductName);
            Assert.Equal(99.99, updated.Price);
        }

    }
}
