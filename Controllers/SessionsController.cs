using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController : ControllerBase
{
    private readonly DataContext _context;

    private static List<Session> Sessions = new List<Session>();
    private static int sessionIdCounter = 1;

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetSessions([FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd)
    {
        var filteredSessions = Sessions.AsQueryable();

        if (periodStart.HasValue)
            filteredSessions = filteredSessions.Where(s => s.StartTime >= periodStart.Value);

        if (periodEnd.HasValue)
            filteredSessions = filteredSessions.Where(s => s.StartTime <= periodEnd.Value);

        return Ok(filteredSessions);
    }

    [HttpGet("{id}")]
    public ActionResult<Session> GetSession(int id)
    {
        var session = _context.Sessions!.Find(id);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateSession(int id, [FromBody] Session updatedSession)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == id);
        if (session == null) return NotFound();

        if (updatedSession.StartTime <= DateTime.UtcNow)
            return BadRequest("Session start time must be in the future.");

        session.MovieId = updatedSession.MovieId;
        session.AuditoriumName = updatedSession.AuditoriumName;
        session.StartTime = updatedSession.StartTime;
        return NoContent();
    }

    [HttpPost]
    public IActionResult CreateSession([FromBody] Session session)
    {
        if (session.StartTime <= DateTime.UtcNow)
            return BadRequest("Session start time must be in the future.");

        session.Id = sessionIdCounter++;
        Sessions.Add(session);
        return CreatedAtAction(nameof(GetSessions), new { id = session.Id }, session);
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
