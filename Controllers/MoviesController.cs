using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly DataContext _context;

    public MoviesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Movie>> GetMovies(string? nam = null)
    {
        var query = _context.Movies!.AsQueryable();

        if (nam != null)
            query = query.Where(x => x.Title != null && x.Title.ToUpper().Contains(nam.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetMovie(int id)
    {
        var movie = _context.Movies!.Find(id);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpPut("{id}")]
    public IActionResult PutMovie(int id, Movie mo)
    {
        var dbMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == mo.Id);
        if (id != mo.Id || dbMovie == null)
        {
            return NotFound();
        }

        _context.Update(mo);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Movie> PostMovie(Movie mo)
    {
        var dbExercise = _context.Movies!.Find(mo.Id);
        if (dbExercise == null)
        {
            _context.Add(mo);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMovie), new { Id = mo.Id }, mo);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        var movie = _context.Sessions!.Find(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Remove(movie);
        _context.SaveChanges();

        return NoContent();
    }
}
