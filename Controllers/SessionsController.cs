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
    public async Task<IActionResult> GetSessions(
        [FromQuery] DateTime? periodStart,
        [FromQuery] DateTime? periodEnd,
        [FromQuery] string? movieTitle)
    {
        var query = _context.Sessions
            .Include(s => s.Movie)
            .AsQueryable();

        if (periodStart.HasValue)
            query = query.Where(s => s.StartTime >= periodStart.Value);

        if (periodEnd.HasValue)
            query = query.Where(s => s.StartTime <= periodEnd.Value);

        if (!string.IsNullOrEmpty(movieTitle))
            query = query.Where(s => s.Movie.Title.ToLower().Contains(movieTitle.ToLower()));

        var result = await query.ToListAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Session>> GetSession(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.Movie)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
            return NotFound();

        return Ok(session);
    }

    [HttpGet("{id}/tickets")]
    public async Task<IActionResult> GetSessionTickets(int id)
    {
        var sessionExists = await _context.Sessions.AnyAsync(s => s.Id == id);
        if (!sessionExists)
            return NotFound();

        var tickets = await _context.Tickets
            .Where(t => t.SessionId == id)
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] Session session)
    {
        if (session.StartTime <= DateTime.UtcNow)
            return BadRequest("Session start time must be in the future.");

        var movieExists = await _context.Movies.AnyAsync(m => m.Id == session.MovieId);
        if (!movieExists)
            return BadRequest("Movie does not exist.");

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSession(int id, [FromBody] Session updatedSession)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
            return NotFound();

        if (updatedSession.StartTime <= DateTime.UtcNow)
            return BadRequest("Session start time must be in the future.");

        session.MovieId = updatedSession.MovieId;
        session.AuditoriumName = updatedSession.AuditoriumName;
        session.StartTime = updatedSession.StartTime;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
            return NotFound();

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
