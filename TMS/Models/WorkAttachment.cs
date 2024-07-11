using System.ComponentModel.DataAnnotations;

namespace TMS_API.Models
{
    public class WorkAttachment
    {
        [Key]
        public int Id { get; set; }

        public int WorkItemId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public DateTime Uploaddate { get; set; }
        public int UploadedBy { get; set; }
    }
}
