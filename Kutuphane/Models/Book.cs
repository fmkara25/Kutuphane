namespace Kutuphane.Models
{
    public enum ReadingStatus { ToRead, Reading, Read, Favorite }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Genre { get; set; } = default!;
        public ReadingStatus Status { get; set; } = ReadingStatus.ToRead;
        public string? Notes { get; set; }
    }
}
