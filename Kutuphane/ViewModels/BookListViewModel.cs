using System.Collections.Generic;
using Kutuphane.Helpers;
using Kutuphane.Models;

namespace Kutuphane.ViewModels
{
    public class BookListViewModel
    {
        // Liste + sayfalama
        public PaginatedList<Book> Books { get; set; } = default!;

        // Dashboard sayıları
        public int TotalCount { get; set; }
        public int ReadingCount { get; set; }      // Okuyorum
        public int ReadCount { get; set; }         // Okudum
        public int ToReadCount { get; set; }       // Okuyacağım
        public int FavoriteCount { get; set; }

        // Filtre/sıralama durumunu korumak için
        public string? Search { get; set; }
        public ReadingStatus? Status { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        // Yeni: Genre filtresi + seçenekler
        public string? Genre { get; set; }
        public List<string> Genres { get; set; } = new();

        // Yeni: sadece favoriler filtresi
        public bool FavoritesOnly { get; set; }
    }
}
