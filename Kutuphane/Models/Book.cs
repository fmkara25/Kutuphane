using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public enum ReadingStatus
    {
        [Display(Name = "Okunacak")] ToRead,
        [Display(Name = "Okunuyor")] Reading,
        [Display(Name = "Okundu")] Read,
        [Display(Name = "Favori")] Favorite
    }

    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı boş bırakılamaz")]
        [StringLength(100, ErrorMessage = "Kitap adı en fazla 100 karakter olabilir")]
        [Display(Name = "Kitap Adı")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Yazar adı boş bırakılamaz")]
        [StringLength(60, ErrorMessage = "Yazar adı en fazla 60 karakter olabilir")]
        [Display(Name = "Yazar")]
        public string Author { get; set; } = default!;

        [StringLength(40, ErrorMessage = "Tür en fazla 40 karakter olabilir")]
        [Display(Name = "Tür")]
        public string? Genre { get; set; }

        [Display(Name = "Durum")]
        public ReadingStatus Status { get; set; } = ReadingStatus.ToRead;

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
    }
}
