using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using AttendanceRecord.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AttendanceRecord.Services;
using Microsoft.Extensions.Primitives;
using System.Data;

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
            //ƒ†[ƒUID
            int userID = 1;
            DataTable dt = new DataTable();
            dt = _dataController.initTimeInput(userID);
            if (dt.Rows.Count > 0)
            {
                StartTime = dt.Rows[0]["start_time"].ToString();
                EndTime = dt.Rows[0]["end_time"].ToString();
                if (StartTime != "")
                {
                    IsClockedIn = true;
                }
                if (EndTime != "")
                {
                    IsClockedOut = true;
                }
            }
            else
            {
                StartTime = "";
                EndTime = "";
                IsClockedIn = false;
                IsClockedOut = false;
            }
        }

        public IActionResult OnPost()
        {
            HttpClient client = _httpClientFactory.CreateClient();

            // BaseUrl‚ðappsettings.json‚©‚çŽæ“¾
            BaseUrl = _configuration["AppSettings:BaseUrl"];
            client.BaseAddress = new Uri(BaseUrl);
            //ƒ†[ƒUID‚ðŒÅ’è‰»
            int userID = 1;
            StringValues action = Request.Form["action"];
            string startTimeStr = Request.Form["startTime"];
            string endTimeStr = Request.Form["endTime"];



            if (action == "clockin" && DateTime.TryParse(startTimeStr, out DateTime startTime))
            {
                int rows = _dataController.UpdateStartTime(userID, startTime);
                if (rows == 2)
                    
                {
                    Message = $"o‹ÎŽžŠÔ‚Í‚·‚Å‚É“o˜^Ï‚Ý‚Å‚·B";
                    IsClockedIn = true;
                }
                else if(rows > 0)
                {

                    Message = $"o‹ÎŽžŠÔ {startTimeStr} ‚ð“o˜^‚µ‚Ü‚µ‚½B";
                    IsClockedIn = true;
                }
                else
                {
                    Message = $"o‹ÎŽžŠÔ‚Ì“o˜^‚ÉŽ¸”s‚à‚µ‚­‚Í“o˜^Ï‚Ý‚Å‚·B";
                    IsClockedIn = true;
                }
            }
            else if (action == "clockout" && DateTime.TryParse(endTimeStr, out DateTime endTime))
            {
                int rows = _dataController.UpdateEndTime(userID, endTime);
                if (rows == 2)

                {
                    Message = $"‘Þ‹ÎŽžŠÔ‚Í‚·‚Å‚É“o˜^Ï‚Ý‚Å‚·B";
                    IsClockedOut = true;
                }
                else if (rows > 0)
                {
                    Message = $"‘Þ‹ÎŽžŠÔ {endTimeStr} ‚ð“o˜^‚µ‚Ü‚µ‚½B";
                    IsClockedOut = true;
                }
                else
                {
                    Message = $"‘Þ‹ÎŽžŠÔ‚Ì“o˜^‚ÉŽ¸”s‚à‚µ‚­‚Í“o˜^Ï‚Ý‚Å‚·B";
                    IsClockedOut = true;
                }
            }
            else
            {
                Message = "ŽžŠÔ‚ÌŒ`Ž®‚ª³‚µ‚­‚ ‚è‚Ü‚¹‚ñB";
            }

            CurrentTime = DateTime.Now.ToString("HH:mm");
            DataTable dt = new DataTable();
            dt = _dataController.initTimeInput(userID);
            if (dt.Rows.Count > 0)
            {
                StartTime = dt.Rows[0]["start_time"].ToString();
                EndTime = dt.Rows[0]["end_time"].ToString();
                if (StartTime != "")
                {
                    IsClockedIn = true;
                }
                if (EndTime != "")
                {
                    IsClockedOut = true;
                }
            }
            return Page();
        }
    }
}
