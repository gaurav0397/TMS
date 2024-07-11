using Microsoft.AspNetCore.Mvc.Rendering;
using TMS_WEB.Models;

namespace TMS_WEB.Viewmodel
{
    public class VMWorkItemUpdate
    {
        public WorkItem WorkItem { get; set; }
        public WorkNote WorkNote { get; set; }
        public IEnumerable<WorkNote> WorkNoteList { get; set; }
        public WorkAttachment WorkAttachment { get; set; }
        public IEnumerable<WorkAttachment> WorkAttachmentList { get; set; }
        public SelectList EmployeeList { get; set; }
        public VMFileUpload fileUploadViewModel { get; set; }
        public List<Employee> EmpList { get; set; }
    }
}
