using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS_API.Data;
using TMS_API.Models;

namespace TMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkAttachmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkAttachmentController(AppDbContext context)
        {
            _context=context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkAttachment>> GetAttachmentByWorkId(int id)
        {
            var attachments = await _context.WorkAttachments
                .Where(a => a.WorkItemId == id)
                .ToListAsync();

            return Ok(attachments);
        }

        [HttpPost]
        public async Task<ActionResult<WorkAttachment>> AddAttachment([FromBody] WorkAttachment workAttachment)
        {
            _context.WorkAttachments.Add(workAttachment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAttachmentByWorkId), new { id = workAttachment.WorkItemId }, workAttachment);

        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var attachment = await _context.WorkAttachments.FindAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            var fileContent = new FileContentResult(attachment.FileData, attachment.ContentType)
            {
                FileDownloadName = attachment.FileName
            };

            return fileContent;
        }


    }
}
