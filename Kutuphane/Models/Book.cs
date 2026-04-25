using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public enum ReadingStatus
    {
        [Display(Name = "Reading")]
        Okuyorum,

        [Display(Name = "Finished")]
        Okudum,

        [Display(Name = "Want to Read")]
        Okuyacağım
    }

    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author cannot be longer than 100 characters.")]
        [Display(Name = "Author")]
        public string Author { get; set; } = string.Empty;

        [StringLength(40, ErrorMessage = "Genre cannot be longer than 40 characters.")]
        [Display(Name = "Genre")]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Reading status is required.")]
        [Display(Name = "Reading Status")]
        public ReadingStatus Status { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot be longer than 1000 characters.")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [Display(Name = "Cover Image URL")]
        public string? CoverImageUrl { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        [Display(Name = "Rating")]
        public double? Rating { get; set; }

        [Range(1, 5000, ErrorMessage = "Page count must be between 1 and 5000.")]
        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        [Display(Name = "Favorite")]
        public bool IsFavorite { get; set; }

        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
        [Display(Name = "Reading Progress")]
        public int? Progress { get; set; }
    }
}