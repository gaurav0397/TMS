using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS_API.Data;
using TMS_API.Models;

namespace TMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkNoteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WorkNoteController(AppDbContext context)
        {
            _context= context;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<WorkNote>>> GetWorkItemNotes(int id)
        {
            var notes = await _context.WorkNotes.Where(x=>x.WorkItemId==id).ToListAsync();
            if (notes == null || !notes.Any())
            {
                return NotFound();
            }
            return Ok(notes);
        }

        [HttpPost]
        public async Task<ActionResult<WorkNote>> AddNotes([FromBody] WorkNote note)
        {
            _context.WorkNotes.Add(note);
            await _context.SaveChangesAsync();
            return Ok(note);
        }

    }
}
