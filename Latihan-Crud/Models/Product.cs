using System.ComponentModel.DataAnnotations;

namespace Latihan_Crud.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama produk wajib diisi!")]
        [StringLength(100, ErrorMessage = "Nama produk tidak boleh lebih dari 100 karakter!")]
        public string Name { get; set; } = string.Empty;

        // Price
        [Required(ErrorMessage = "Harga produk wajib diisi!")]
        [Range(0, 100000000, ErrorMessage = "Harga tidak boleh minus! dan hanya sampai 100jt")]
        public decimal Price { get; set; }
    }
}
