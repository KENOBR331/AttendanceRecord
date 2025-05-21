using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using AttendanceRecord.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AttendanceRecord.Services;

namespace AttendanceRecord.Pages
{
    public class TimeInputModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly DataController _dataController;
        [BindProperty]
        public string StartTime { get; set; }

        [BindProperty]
        public string EndTime { get; set; }

        public string Message { get; set; }
        public string CurrentTime { get; private set; }
        public bool IsClockedIn { get; set; }
        public bool IsClockedOut { get; set; }
        public string BaseUrl { get; set; }

        public TimeInputModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, DataController dataController)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dataController = dataController;
        }

        public void OnGet()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        public IActionResult OnPost()
        {
            var client = _httpClientFactory.CreateClient();

            // BaseUrl��appsettings.json����擾
            BaseUrl = _configuration["AppSettings:BaseUrl"];
            client.BaseAddress = new Uri(BaseUrl);

            var action = Request.Form["action"];
            var startTimeStr = Request.Form["startTime"];
            var endTimeStr = Request.Form["endTime"];



            if (action == "clockin" && DateTime.TryParse(startTimeStr, out var startTime))
            {
                int rows = _dataController.UpdateStartTime(1, startTime);
                if (rows == 2)
                    
                {
                    Message = $"�o�Ύ��Ԃ͂��łɓo�^�ς݂ł��B";
                }
                else if(rows > 0)
                {

                    Message = $"�o�Ύ��� {startTimeStr} ��o�^���܂����B";
                    IsClockedIn = true;
                }
                else
                {
                    Message = $"�o�Ύ��Ԃ̓o�^�Ɏ��s�������͓o�^�ς݂ł��B";
                }
            }
            else if (action == "clockout" && DateTime.TryParse(endTimeStr, out var endTime))
            {
                int rows = _dataController.UpdateEndTime(1, endTime);
                if (rows > 0)
                {
                    Message = $"�ދΎ��� {endTimeStr} ��o�^���܂����B";
                    IsClockedOut = true;
                }
                else
                {
                    Message = $"�ދΎ��Ԃ̓o�^�Ɏ��s�������͓o�^�ς݂ł��B";
                }
            }
            else
            {
                Message = "���Ԃ̌`��������������܂���B";
            }

            CurrentTime = DateTime.Now.ToString("HH:mm");
            return Page();
        }
    }
}
