using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController : ControllerBase
{
    private readonly DataContext _context;

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions([FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd)
    {
        var query = _context.Sessions!.AsQueryable();

        if (periodStart.HasValue)
            query = query.Where(s => s.StartTime >= periodStart.Value);

        if (periodEnd.HasValue)
            query = query.Where(s => s.StartTime <= periodEnd.Value);

        var sessions = await query.ToListAsync();
        return Ok(sessions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Session>> GetSession(int id)
    {
        var session = await _context.Sessions!.FindAsync(id);

        if (session == null)
            return NotFound();

        return Ok(session);
    }

    [HttpPost]
    public IActionResult CreateSession([FromBody] Session session)
    {
        if (session.StartTime <= DateTime.UtcNow)
            return BadRequest("Session start time must be in the future.");

        var movieExists = _context.Movies.Any(m => m.Id == session.MovieId);
        if (!movieExists)
            return BadRequest("Movie with given ID does not exist.");

        _context.Sessions.Add(session);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSession(int id, [FromBody] Session updatedSession)
    {
        if (updatedSession.StartTime <= DateTime.UtcNow)
            return BadRequest("Session start time must be in the future.");

        var session = await _context.Sessions!.FindAsync(id);
        if (session == null)
            return NotFound();

        session.MovieId = updatedSession.MovieId;
        session.AuditoriumName = updatedSession.AuditoriumName;
        session.StartTime = updatedSession.StartTime;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(int id)
    {
        var session = await _context.Sessions!.FindAsync(id);
        if (session == null)
            return NotFound();

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return Ok();
    }
}