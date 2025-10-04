using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;
using Kutuphane.Helpers;
using Kutuphane.ViewModels;

namespace Kutuphane.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Books
        // Query string: ?search=&status=&sort=&page=&pageSize=
        public async Task<IActionResult> Index(
            string? search,
            ReadingStatus? status,
            string? sort,
            int page = 1,
            int pageSize = 9)
        {
            // ---- Dashboard sayaçları (ardışık) ----
            var totalCount = await _context.Books.AsNoTracking().CountAsync();
            var readingCount = await _context.Books.AsNoTracking().CountAsync(b => b.Status == ReadingStatus.Okuyorum);
            var readCount = await _context.Books.AsNoTracking().CountAsync(b => b.Status == ReadingStatus.Okudum);
            var toReadCount = await _context.Books.AsNoTracking().CountAsync(b => b.Status == ReadingStatus.Okuyacağım);
            var favoriteCount = await _context.Books.AsNoTracking().CountAsync(b => b.IsFavorite);

            // ---- Liste sorgusu (filtre + sıralama + sayfalama) ----
            var q = _context.Books.AsNoTracking().AsQueryable();

            // Arama
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(b =>
                    b.Title.ToLower().Contains(s) ||
                    b.Author.ToLower().Contains(s) ||
                    (b.Genre != null && b.Genre.ToLower().Contains(s)));
            }

            // Durum filtresi
            if (status.HasValue)
                q = q.Where(b => b.Status == status.Value);

            // Sıralama
            // sort: title, title_desc, author, author_desc, rating, rating_desc
            q = sort switch
            {
                "title_desc" => q.OrderByDescending(b => b.Title),
                "author" => q.OrderBy(b => b.Author),
                "author_desc" => q.OrderByDescending(b => b.Author),
                "rating" => q.OrderBy(b => b.Rating),
                "rating_desc" => q.OrderByDescending(b => b.Rating),
                _ => q.OrderBy(b => b.Title)
            };

            var paged = await PaginatedList<Book>.CreateAsync(q, page, pageSize);

            var vm = new BookListViewModel
            {
                Books = paged,
                TotalCount = totalCount,
                ReadingCount = readingCount,
                ReadCount = readCount,
                ToReadCount = toReadCount,
                FavoriteCount = favoriteCount,
                Search = search,
                Status = status,
                Sort = sort,
                Page = page,
                PageSize = pageSize
            };

            return View(vm);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create() => View();

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Status,Notes,CoverImageUrl,Rating,PageCount,IsFavorite,Progress")] Book book)
        {
            if (!ModelState.IsValid) return View(book);
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,Status,Notes,CoverImageUrl,Rating,PageCount,IsFavorite,Progress")] Book book)
        {
            if (id != book.Id) return NotFound();
            if (!ModelState.IsValid) return View(book);

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Books.AsNoTracking().AnyAsync(e => e.Id == book.Id);
                if (!exists) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Books/ToggleFavorite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int id, string? search, ReadingStatus? status, string? sort, int page = 1, int pageSize = 9)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.IsFavorite = !book.IsFavorite;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { search, status, sort, page, pageSize });
        }

        // POST: Books/UpdateProgress/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int id, int progress, string? search, ReadingStatus? status, string? sort, int page = 1, int pageSize = 9)
        {
            if (progress < 0) progress = 0;
            if (progress > 100) progress = 100;

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Progress = progress;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { search, status, sort, page, pageSize });
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null) _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
