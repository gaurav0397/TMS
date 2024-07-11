using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TMS_WEB.Models;

namespace TMS_WEB.Services
{
    public class APIservice
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public APIservice(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }
        #region Get Work Items
        public async Task<List<WorkItem>> GetAllWorkItems()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}WorkItem");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var workItems = JsonSerializer.Deserialize<List<WorkItem>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return workItems;
        }

        public async Task<List<WorkItem>> GetWorkItemForEmpId(int empId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}WorkItem");

            var content = await response.Content.ReadAsStringAsync();
            var allWorkItems = JsonSerializer.Deserialize<List<WorkItem>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var WorkItem = allWorkItems.Where(i => i.AssignedTo == empId).ToList();

            return WorkItem;
        }

        public async Task<List<WorkItem>> GetWorkItemForManager(int empId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}WorkItem");

            var content = await response.Content.ReadAsStringAsync();
            var allWorkItems = JsonSerializer.Deserialize<List<WorkItem>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Employee manager = await GetEmployeeById(empId);

            var WorkItem = allWorkItems.Where(i =>i.AssignedTeamId== manager.TeamId).ToList();

            return WorkItem;
        }

        public async Task<WorkItem> GetWorkItemById(int id) 
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}Workitem/{id}");
            
            var content = await response.Content.ReadAsStringAsync();
            var workItem = JsonSerializer.Deserialize<WorkItem>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return workItem;
        }
        #endregion

        #region Create Work Item
        public async Task CreateWorkItem(WorkItem workItem)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}WorkItem", workItem);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateWorkItem(WorkItem workItem)
        {
            var url = $"{_apiBaseUrl}WorkItem/{workItem.Id}"; 

            var response = await _httpClient.PutAsJsonAsync(url, workItem);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Get Employee and Team
        public async Task<List<Employee>> GetEmployees()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}Employee");

            var content = await response.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<Employee>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return employees;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}Employee/{id}");
            var content = await response.Content.ReadAsStringAsync();
            var employee = JsonSerializer.Deserialize<Employee>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return employee;
        }

        public async Task<int> GetTeamID(int empId)
        {
            Employee employee = await GetEmployeeById(empId);
            return Convert.ToInt32(employee.TeamId);
        }

        #endregion

        #region Work Notes
        public async Task AddWorkNote(WorkNote workNote)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}WorkNote", workNote);
        }

        public async Task<List<WorkNote>> GetWorkNotes(int id)
        {
            var url = $"{_apiBaseUrl}WorkNote/{id}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<WorkNote>(); 
                }

                var content = await response.Content.ReadAsStringAsync();
                var workNotes = JsonSerializer.Deserialize<List<WorkNote>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return workNotes ?? new List<WorkNote>();
            }
            catch (HttpRequestException)
            {
                return new List<WorkNote>(); 
            }
        }

        #endregion

        #region Work Attachments
        public async Task<List<WorkAttachment>> GetAttachments(int id)
        {
            var url = $"{_apiBaseUrl}WorkAttachment/{id}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<WorkAttachment>();
                }

                var content = await response.Content.ReadAsStringAsync();

                var attachments = JsonSerializer.Deserialize<List<WorkAttachment>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                {
                    new ByteArrayConverter()
                }
                });

                return attachments ?? new List<WorkAttachment>();
            }
            catch (HttpRequestException)
            {
                return new List<WorkAttachment>();
            }
        }

        public async Task AddWorkAttachment(WorkAttachment workAttachment)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new ByteArrayConverter() }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(workAttachment, options), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}WorkAttachment", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
            }

            response.EnsureSuccessStatusCode();
        }
        public async Task<(byte[] FileData, string ContentType, string FileName)> DownloadAttachmentAsync(int id)
        {
            var url = $"{_apiBaseUrl}WorkAttachment/download/{id}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to download attachment. Status code: {response.StatusCode}");
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;

            var fileData = await response.Content.ReadAsByteArrayAsync();

            var contentDisposition = response.Content.Headers.ContentDisposition;
            string fileName = null;
            if (contentDisposition != null)
            {
                fileName = contentDisposition.FileName;
            }

            return (fileData, contentType, fileName);
        }
        #endregion


    }

    #region Helper class for attachment
    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var base64String = reader.GetString();
            return string.IsNullOrEmpty(base64String) ? Array.Empty<byte>() : Convert.FromBase64String(base64String);
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteBase64StringValue(value);
        }
    }
    #endregion
}
