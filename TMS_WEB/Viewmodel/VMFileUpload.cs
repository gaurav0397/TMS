using System.ComponentModel.DataAnnotations;

namespace TMS_WEB.Viewmodel
{
    public class VMFileUpload
    {

        public int Id { get; set; }
        public int WorkItemId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public IFormFile Filedata { get; set; }
        public DateTime Uploaddate { get; set; }
        public int UploadedBy { get; set; }
    }
}
