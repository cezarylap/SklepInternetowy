using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using ProductSklepInternetowyUI.Models; // <- dostosuj namespace

namespace SklepInternetowy.Tests.UnitTests
{
    public class ProductValidationTests
    {
        // Pomocnicza metoda do uruchamiania walidacji modelu
        private IList<ValidationResult> ValidateModel(object model)
        {
            var ctx = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }

        [Fact]
        public void Product_WithValidData_ShouldHaveNoValidationErrors()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Valid Product",
                Price = 9.99,
                AuthorName = "Tester" 
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("", 9.99)]
        [InlineData(null, 9.99)]
        [InlineData("Valid", 0.0)]
        [InlineData("Valid", -5.0)]
        public void Product_WithInvalidData_ShouldHaveValidationErrors(string? name, double price)
        {
            // Arrange
            var product = new Product
            {
                ProductName = name,
                Price = price
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.NotEmpty(results); // oczekujemy co najmniej jednego błędu
        }
        [Fact]
        public void Product_WithTooLongName_ShouldFailValidation()
        {
            // Arrange
            var product = new Product
            {
                ProductName = new string('X', 100), // 100 znaków
                Price = 9.99
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ProductName"));
        }

    }
}