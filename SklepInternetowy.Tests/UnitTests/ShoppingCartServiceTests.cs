using System.Collections.Generic;
using Xunit;
using ProductSklepInternetowyUI.Models;

namespace SklepInternetowy.UnitTests
{
    public class ShoppingCartServiceTests
    {
        [Fact]
        public void CalculateTotal_ShouldReturnSumOfProductPricesTimesQuantity()
        {
            // Arrange
            var items = new List<CartDetail>
            {
                new CartDetail { Product = new Product { Id = 1, ProductName = "P1", Price = 5 }, Quantity = 2 },
                new CartDetail { Product = new Product { Id = 2, ProductName = "P2", Price = 3 }, Quantity = 1 }
            };

            // Act
            var total = 0.0;
            foreach (var item in items)
            {
                total += item.Product.Price * item.Quantity;
            }

            // Assert
            Assert.Equal(13.0, total);
        }
        [Fact]
        public void CalculateTotal_WithEmptyCart_ShouldReturnZero()
        {
            // Arrange
            var items = new List<CartDetail>();

            // Act
            var total = 0.0;
            foreach (var item in items)
            {
                total += item.Product.Price * item.Quantity;
            }

            // Assert
            Assert.Equal(0.0, total);
        }

    }
}
