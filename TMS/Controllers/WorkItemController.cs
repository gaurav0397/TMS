using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS_API.Data;
using TMS_API.Models;

namespace TMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkItemController(AppDbContext context)
        {
            _context=context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkItem>>> GetAllTasks()
        {
            return await _context.WorkItems.ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<WorkItem>> AddTask([FromBody] WorkItem workItem)
        {
            _context.WorkItems.Add(workItem);
            await _context.SaveChangesAsync();
            return Ok(workItem);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItem>> GetTaskById(int id)
        {
            var task = await _context.WorkItems.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] WorkItem task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool TaskExists(int id)
        {
            return _context.WorkItems.Any(e => e.Id == id);
        }
    }
}
