namespace TMS_WEB.Models
{
    public class WorkAttachment
    {
        public int Id { get; set; }

        public int WorkItemId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public DateTime Uploaddate { get; set; }
        public int UploadedBy { get; set; }
    }
}
