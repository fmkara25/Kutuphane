using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kutuphane.Models
{
    public enum ReadingStatus
    {
        Okuyorum,
        Okudum,
        Okuyacağım
    }

    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [StringLength(40)]
        public string? Genre { get; set; }

        [Required]
        public ReadingStatus Status { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Url(ErrorMessage = "Lütfen geçerli bir URL girin.")]
        [Display(Name = "Kapak Görseli URL")]
        public string? CoverImageUrl { get; set; }

        [Range(0, 5)]
        [Display(Name = "Puan (0-5)")]
        public double? Rating { get; set; }

        [Range(1, 5000)]
        [Display(Name = "Sayfa Sayısı")]
        public int? PageCount { get; set; }

        [Display(Name = "Favori mi?")]
        public bool IsFavorite { get; set; }

        [Range(0, 100)]
        [Display(Name = "Okuma İlerlemesi (%)")]
        public int? Progress { get; set; }
    }
}
