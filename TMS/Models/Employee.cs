using System.ComponentModel.DataAnnotations;

namespace TMS_API.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public int? TeamId { get; set; }
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }
    }
}
