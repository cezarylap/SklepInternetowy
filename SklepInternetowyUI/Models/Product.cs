using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductSklepInternetowyUI.Models
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? ProductName { get; set; } //nazwa produktu,

        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; } //autor lub marka
        [Required]
        public double Price { get; set; } //cena
        public string? Image { get; set; } //ścieżka do zdjęcia produktu,
        [Required]
        public int GenreId { get; set; } //powiązanie z kategorią produktu
        public Genre Genre { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }
        public Stock Stock { get; set; }

        [NotMapped]
        public string GenreName { get; set; } //wykorzystywana tylko do wyświetlania kategorii,
        [NotMapped]
        public int Quantity { get; set; } //liczba produktów np. w koszyku lub w raporcie.
    }
}
