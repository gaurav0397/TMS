using System.ComponentModel.DataAnnotations;

namespace TMS_API.Models
{
    public class WorkNote
    {
        [Key]
        public int Id { get; set; }
        public int WorkItemId { get; set; }
        public string Content { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}
