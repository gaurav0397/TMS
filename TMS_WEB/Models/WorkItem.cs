using System.ComponentModel.DataAnnotations;

namespace TMS_WEB.Models
{
    public class WorkItem
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime Duedate { get; set; }
        public int AssignedTo { get; set; }
        public int AssignedTeamId { get; set; }


        public Employee employee { get; set; }
    }
}
