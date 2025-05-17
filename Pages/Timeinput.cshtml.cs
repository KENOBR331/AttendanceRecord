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

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();
            BaseUrl = _configuration["AppSettings:BaseUrl"];
            client.BaseAddress = new Uri(BaseUrl);

            var action = Request.Form["action"];
            var startTimeStr = Request.Form["startTime"];
            var endTimeStr = Request.Form["endTime"];



            if (action == "clockin" && DateTime.TryParse(startTimeStr, out var startTime))
            {
                int rows = _dataController.UpdateStartTime(1, startTime);
                if (rows > 0)
                {
                    Message = $"èoãŒéûä‘ {startTimeStr} Çìoò^ÇµÇ‹ÇµÇΩÅB";
                    IsClockedIn = true;
                }
                else
                {
                    Message = $"èoãŒéûä‘ÇÃìoò^Ç…é∏îsÇ‡ÇµÇ≠ÇÕìoò^çœÇ›Ç≈Ç∑ÅB";
                }
            }
            else if (action == "clockout" && DateTime.TryParse(endTimeStr, out var endTime))
            {
                int rows = _dataController.UpdateEndTime(1, endTime);
                if (rows > 0)
                {
                    Message = $"ëﬁãŒéûä‘ {endTimeStr} Çìoò^ÇµÇ‹ÇµÇΩÅB";
                    IsClockedOut = true;
                }
                else
                {
                    Message = $"ëﬁãŒéûä‘ÇÃìoò^Ç…é∏îsÇ‡ÇµÇ≠ÇÕìoò^çœÇ›Ç≈Ç∑ÅB";
                }
            }
            else
            {
                Message = "éûä‘ÇÃå`éÆÇ™ê≥ÇµÇ≠Ç†ÇËÇ‹ÇπÇÒÅB";
            }

            CurrentTime = DateTime.Now.ToString("HH:mm");
            return Page();
        }
    }
}
