using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_API.Models
{
    public class WorkItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]     //for sqllite
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime Duedate { get; set; }
        public int AssignedTo { get; set; }
        public int AssignedTeamId { get; set; }
    }
}
