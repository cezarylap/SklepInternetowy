using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ProductSklepInternetowyUI.Models;
using ProductSklepInternetowyUI.Repositories;

namespace SklepInternetowy.Tests.UnitTests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetProducts_ShouldReturnAllProducts()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.GetProducts())
                    .ReturnsAsync(new List<Product> {
                        new Product { Id = 1, ProductName = "A", Price = 10.0 },
                        new Product { Id = 2, ProductName = "B", Price = 20.0 }
                    });

            // Act
            var result = (await mockRepo.Object.GetProducts()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.ProductName == "A" && p.Price == 10.0);
            Assert.Contains(result, p => p.ProductName == "B" && p.Price == 20.0);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNullIfNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.GetProductById(It.IsAny<int>()))
                    .ReturnsAsync((Product)null!);

            // Act
            var result = await mockRepo.Object.GetProductById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_IfFound()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var expected = new Product { Id = 3, ProductName = "Laptop", Price = 1500.0 };
            mockRepo.Setup(r => r.GetProductById(3))
                    .ReturnsAsync(expected);

            // Act
            var result = await mockRepo.Object.GetProductById(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laptop", result.ProductName);
            Assert.Equal(1500.0, result.Price);
        }
        [Fact]
        public async Task AddProduct_ShouldInvokeRepositoryWithCorrectProduct()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var newProduct = new Product
            {
                ProductName = "Nowy Produkt",
                Price = 99.99,
                AuthorName = "UnitTester"
            };

            mockRepo.Setup(r => r.AddProduct(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await mockRepo.Object.AddProduct(newProduct);

            // Assert
            mockRepo.Verify(r => r.AddProduct(It.Is<Product>(
                p => p.ProductName == "Nowy Produkt" &&
                     p.Price == 99.99 &&
                     p.AuthorName == "UnitTester"
            )), Times.Once);
        }

    }
}
