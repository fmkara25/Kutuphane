namespace Kutuphane.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string? Genre { get; set; }
        public string? CoverImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? PageCount { get; set; }
        public bool IsFavorite { get; set; }
        public int? Progress { get; set; }
        public string Status { get; set; } = ""; // enum string olarak
        public string? Notes { get; set; }
    }
}
