using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly DataContext _context;

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Session>> GetSessions(string? nam = null)
    {
        var query = _context.Sessions!.AsQueryable();

        if (nam != null)
            query = query.Where(x => x.AuditoriumName != null && x.AuditoriumName.ToUpper().Contains(nam.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetSession(int id)
    {
        var session = _context.Sessions!.Find(id);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPut("{id}")]
    public IActionResult PutSession(int id, Session ses)
    {
        var dbSession = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == ses.Id);
        if (id != ses.Id || dbSession == null)
        {
            return NotFound();
        }

        _context.Update(ses);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Session> PostSession(Session ses)
    {
        var dbExercise = _context.Sessions!.Find(ses.Id);
        if (dbExercise == null)
        {
            _context.Add(ses);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSession), new { Id = ses.Id }, ses);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSession(int id)
    {
        var session = _context.Sessions!.Find(id);
        if (session == null)
        {
            return NotFound();
        }

        _context.Remove(session);
        _context.SaveChanges();

        return NoContent();
    }
}
