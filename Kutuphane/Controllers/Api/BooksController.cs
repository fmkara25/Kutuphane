using System.Linq;
using System.Threading.Tasks;
using Kutuphane.Data;
using Kutuphane.Dtos;
using Kutuphane.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BooksController(AppDbContext db) => _db = db;

        // GET /api/books?search=&status=&genre=&favoritesOnly=false&sort=&page=1&pageSize=9
        [HttpGet]
        public async Task<IActionResult> GetList(
            string? search, ReadingStatus? status, string? genre,
            bool favoritesOnly = false, string? sort = "",
            int page = 1, int pageSize = 9)
        {
            var q = _db.Books.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(b =>
                    b.Title.ToLower().Contains(s) ||
                    b.Author.ToLower().Contains(s) ||
                    (b.Genre != null && b.Genre.ToLower().Contains(s)));
            }
            if (status.HasValue) q = q.Where(b => b.Status == status.Value);
            if (!string.IsNullOrWhiteSpace(genre)) q = q.Where(b => b.Genre == genre);
            if (favoritesOnly) q = q.Where(b => b.IsFavorite);

            q = sort switch
            {
                "title_desc" => q.OrderByDescending(b => b.Title),
                "author" => q.OrderBy(b => b.Author),
                "author_desc" => q.OrderByDescending(b => b.Author),
                "rating" => q.OrderBy(b => b.Rating),
                "rating_desc" => q.OrderByDescending(b => b.Rating),
                _ => q.OrderBy(b => b.Title)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    CoverImageUrl = b.CoverImageUrl,
                    Rating = b.Rating,
                    PageCount = b.PageCount,
                    IsFavorite = b.IsFavorite,
                    Progress = b.Progress,
                    Status = b.Status.ToString(),
                    Notes = b.Notes
                })
                .ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                totalPages = (int)System.Math.Ceiling(total / (double)pageSize),
                items
            });
        }

        // GET /api/books/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var b = await _db.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (b == null) return NotFound();

            var dto = new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genre = b.Genre,
                CoverImageUrl = b.CoverImageUrl,
                Rating = b.Rating,
                PageCount = b.PageCount,
                IsFavorite = b.IsFavorite,
                Progress = b.Progress,
                Status = b.Status.ToString(),
                Notes = b.Notes
            };
            return Ok(dto);
        }

        // POST /api/books
        [HttpPost]
        public async Task<IActionResult> Create(BookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
                return BadRequest("Title and Author are required");

            // enum parse (dto.Status boş ise Okuyacağım varsayıyoruz)
            var status = ReadingStatus.Okuyacağım;
            if (!string.IsNullOrWhiteSpace(dto.Status) &&
                System.Enum.TryParse<ReadingStatus>(dto.Status, out var parsed))
                status = parsed;

            var b = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                CoverImageUrl = dto.CoverImageUrl,
                Rating = dto.Rating,
                PageCount = dto.PageCount,
                IsFavorite = dto.IsFavorite,
                Progress = dto.Progress,
                Status = status,
                Notes = dto.Notes
            };
            _db.Books.Add(b);
            await _db.SaveChangesAsync();
            dto.Id = b.Id;
            dto.Status = b.Status.ToString();
            return CreatedAtAction(nameof(Get), new { id = b.Id }, dto);
        }

        // PUT /api/books/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, BookDto dto)
        {
            var b = await _db.Books.FindAsync(id);
            if (b == null) return NotFound();

            b.Title = dto.Title ?? b.Title;
            b.Author = dto.Author ?? b.Author;
            b.Genre = dto.Genre;
            b.CoverImageUrl = dto.CoverImageUrl;
            b.Rating = dto.Rating;
            b.PageCount = dto.PageCount;
            b.IsFavorite = dto.IsFavorite;
            b.Progress = dto.Progress;

            if (!string.IsNullOrWhiteSpace(dto.Status) &&
                System.Enum.TryParse<ReadingStatus>(dto.Status, out var parsed))
                b.Status = parsed;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH /api/books/5/favorite
        [HttpPatch("{id:int}/favorite")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var b = await _db.Books.FindAsync(id);
            if (b == null) return NotFound();
            b.IsFavorite = !b.IsFavorite;
            await _db.SaveChangesAsync();
            return Ok(new { b.Id, b.IsFavorite });
        }

        // PATCH /api/books/5/progress?value=80
        [HttpPatch("{id:int}/progress")]
        public async Task<IActionResult> SetProgress(int id, [FromQuery] int value)
        {
            var b = await _db.Books.FindAsync(id);
            if (b == null) return NotFound();
            if (value < 0) value = 0;
            if (value > 100) value = 100;
            b.Progress = value;
            await _db.SaveChangesAsync();
            return Ok(new { b.Id, b.Progress });
        }

        // DELETE /api/books/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await _db.Books.FindAsync(id);
            if (b == null) return NotFound();
            _db.Books.Remove(b);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/books/genres  (UI için tür listesi)
        [HttpGet("genres")]
        public async Task<IActionResult> Genres()
        {
            var genres = await _db.Books.AsNoTracking()
                .Where(x => x.Genre != null && x.Genre != "")
                .Select(x => x.Genre!)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
            return Ok(genres);
        }
    }
}
