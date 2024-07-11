using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using TMS_WEB.Models;
using TMS_WEB.Services;
using TMS_WEB.Viewmodel;

namespace TMS_WEB.Controllers
{
    [Route("[controller]")]
    public class WorkItemController : Controller
    {
        private readonly APIservice _apiService;

        public WorkItemController(APIservice apiService)
        {
            _apiService = apiService;
        }
        #region Public Methods

        #region Index
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            int employeeID =Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            var employee = await _apiService.GetEmployeeById(employeeID);
            dynamic workItems;

            if(employee.IsAdmin)
            {
                workItems = await _apiService.GetAllWorkItems();
            }
            else if (employee.IsManager)
            {
                 workItems = await _apiService.GetWorkItemForManager(employeeID);
            }
            else
            {
                workItems = await _apiService.GetWorkItemForEmpId(employeeID);
            }
            
            

            foreach (var item in workItems)
            {
                if (item.employee == null)
                {
                    item.employee = new Employee(); // Initialize with a new Employee object
                }

                item.employee.EmployeeName = await GetEmployeeName(item.AssignedTo);
            }

            return View(workItems);
        }

        #endregion

        #region Create WorkItem

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.EmployeeList = new SelectList(await _apiService.GetEmployees(), "Id", "EmployeeName");
            var model = new WorkItem
            {
                CreationDate = DateTime.Now,
                Status = 1
                //employee = _apiService.GetEmployeeById()
            
            };
            return View(model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(WorkItem workItem)
        {
            Employee employee = await _apiService.GetEmployeeById(workItem.AssignedTo);
            workItem.AssignedTeamId = Convert.ToInt32(employee.TeamId);
            await _apiService.CreateWorkItem(workItem);
            return RedirectToAction(nameof(Index));
        }

        

        #endregion

        #region Update WorkItem
        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var employees = await _apiService.GetEmployees();
            var workNoteList = await _apiService.GetWorkNotes(id);
            var workItem = await _apiService.GetWorkItemById(id);
            var workAttchmentList = await _apiService.GetAttachments(id);
            var EmployeeList = _apiService.GetEmployees();


            var viewModel = new VMWorkItemUpdate
            {
                WorkItem = workItem,
                WorkNoteList = workNoteList,
                EmployeeList = new SelectList(employees, "Id", "EmployeeName"),
                WorkAttachmentList = workAttchmentList,
                EmpList = await EmployeeList

            };

            return View(viewModel);
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(VMWorkItemUpdate workItemUpdateViewModel)
        {
            workItemUpdateViewModel.WorkNote.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            workItemUpdateViewModel.WorkNote.UpdateDate = DateTime.Now;
            workItemUpdateViewModel.fileUploadViewModel.UploadedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            workItemUpdateViewModel.fileUploadViewModel.Uploaddate =DateTime.Now;
            workItemUpdateViewModel.WorkItem.AssignedTeamId = await _apiService.GetTeamID(workItemUpdateViewModel.WorkItem.AssignedTo);

            await _apiService.UpdateWorkItem(workItemUpdateViewModel.WorkItem);
            await _apiService.AddWorkNote(workItemUpdateViewModel.WorkNote);
            await ProcessFileUpload(workItemUpdateViewModel.fileUploadViewModel);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region DownloadAttachment
        [HttpGet("DownloadAttachment/{id}")]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var (fileData, contentType, fileName) = await _apiService.DownloadAttachmentAsync(id);
            if (fileData == null || contentType == null)
            {
                return NotFound();
            }
            return File(fileData, contentType, fileName);
        }
        #endregion

        #endregion

        #region Private Methods
        private async Task<string> GetEmployeeName(int assignedTo)
        {
            var empList = await _apiService.GetEmployees();
            var empName = empList.FirstOrDefault(emp => emp.Id == assignedTo)?.EmployeeName;
            return empName ?? "Employee Not Found";

        }

        private async Task ProcessFileUpload(VMFileUpload fileUploadViewModel)
        {
            if (fileUploadViewModel.Filedata != null && fileUploadViewModel.Filedata.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fileUploadViewModel.Filedata.CopyToAsync(memoryStream);
                    var fileEntity = new WorkAttachment
                    {
                        FileName = fileUploadViewModel.FileName,
                        FileData = memoryStream.ToArray(),
                        WorkItemId = fileUploadViewModel.WorkItemId,
                        Uploaddate = fileUploadViewModel.Uploaddate,
                        UploadedBy = fileUploadViewModel.UploadedBy,
                        ContentType = fileUploadViewModel.Filedata.ContentType
                    };

                    await _apiService.AddWorkAttachment(fileEntity);
                }
            }
        }

        private async  Task<int> GetTeamId(int employeeId)
        {
            return await _apiService.GetTeamID(employeeId);
        }
        #endregion
    }
}
