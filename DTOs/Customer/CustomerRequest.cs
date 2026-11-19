using System.ComponentModel.DataAnnotations;

namespace HikeRent.DTOs.Customer
{
    public class CustomerRequest
    {
        [Required(ErrorMessage = "Nama Lengkap wajib diisi.")]
        [MaxLength(100, ErrorMessage = "Nama Lengkap maksimal 100 karakter.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [MaxLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "nomor telepon wajib diisi.")]
        [MaxLength(20, ErrorMessage = "nomor telepon maksimal 100 karakter.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alamat wajib diisi.")]
        [MaxLength(255, ErrorMessage = "Alamat maksimal 100 karakter.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "NIK wajib diisi.")]
        // Harus pakai KTP
        [StringLength(16, MinimumLength = 16, ErrorMessage = "NIK Wajib memiliki 16 karakter/angka")]
        public string IdentityNumber { get; set; } = string.Empty;
        
        //public DateTimeOffset CreatedAt { get; set; }
    }
}
