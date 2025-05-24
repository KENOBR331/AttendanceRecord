using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using AttendanceRecord.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AttendanceRecord.Services;
using Microsoft.Extensions.Primitives;

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
            HttpClient client = _httpClientFactory.CreateClient();

            // BaseUrl‚ğappsettings.json‚©‚çæ“¾
            BaseUrl = _configuration["AppSettings:BaseUrl"];
            client.BaseAddress = new Uri(BaseUrl);
            //ƒ†[ƒUID‚ğŒÅ’è‰»
            int userID = 1;
            StringValues action = Request.Form["action"];
            string startTimeStr = Request.Form["startTime"];
            string endTimeStr = Request.Form["endTime"];



            if (action == "clockin" && DateTime.TryParse(startTimeStr, out DateTime startTime))
            {
                int rows = _dataController.UpdateStartTime(userID, startTime);
                if (rows == 2)
                    
                {
                    Message = $"o‹ÎŠÔ‚Í‚·‚Å‚É“o˜^Ï‚İ‚Å‚·B";
                    IsClockedIn = true;
                }
                else if(rows > 0)
                {

                    Message = $"o‹ÎŠÔ {startTimeStr} ‚ğ“o˜^‚µ‚Ü‚µ‚½B";
                    IsClockedIn = true;
                }
                else
                {
                    Message = $"o‹ÎŠÔ‚Ì“o˜^‚É¸”s‚à‚µ‚­‚Í“o˜^Ï‚İ‚Å‚·B";
                    IsClockedIn = true;
                }
            }
            else if (action == "clockout" && DateTime.TryParse(endTimeStr, out DateTime endTime))
            {
                int rows = _dataController.UpdateEndTime(userID, endTime);
                if (rows == 2)

                {
                    Message = $"‘Ş‹ÎŠÔ‚Í‚·‚Å‚É“o˜^Ï‚İ‚Å‚·B";
                    IsClockedOut = true;
                }
                else if (rows > 0)
                {
                    Message = $"‘Ş‹ÎŠÔ {endTimeStr} ‚ğ“o˜^‚µ‚Ü‚µ‚½B";
                    IsClockedOut = true;
                }
                else
                {
                    Message = $"‘Ş‹ÎŠÔ‚Ì“o˜^‚É¸”s‚à‚µ‚­‚Í“o˜^Ï‚İ‚Å‚·B";
                    IsClockedOut = true;
                }
            }
            else
            {
                Message = "ŠÔ‚ÌŒ`®‚ª³‚µ‚­‚ ‚è‚Ü‚¹‚ñB";
            }

            CurrentTime = DateTime.Now.ToString("HH:mm");
            return Page();
        }
    }
}
