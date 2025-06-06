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

        //ここの時間をデータがあるときはそちらの値に変更する(StartとEndに分けること)
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
            //ユーザID
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

            // BaseUrlをappsettings.jsonから取得
            BaseUrl = _configuration["AppSettings:BaseUrl"];
            client.BaseAddress = new Uri(BaseUrl);
            //ユーザIDを固定化
            int userID = 1;
            StringValues action = Request.Form["action"];
            string startTimeStr = Request.Form["startTime"];
            string endTimeStr = Request.Form["endTime"];



            if (action == "clockin" && DateTime.TryParse(startTimeStr, out DateTime startTime))
            {
                int rows = _dataController.UpdateStartTime(userID, startTime);
                if (rows == 2)
                    
                {
                    Message = $"出勤時間はすでに登録済みです。";
                    IsClockedIn = true;
                }
                else if(rows > 0)
                {

                    Message = $"出勤時間 {startTimeStr} を登録しました。";
                    IsClockedIn = true;
                }
                else
                {
                    Message = $"出勤時間の登録に失敗もしくは登録済みです。";
                    IsClockedIn = true;
                }
            }
            else if (action == "clockout" && DateTime.TryParse(endTimeStr, out DateTime endTime))
            {
                int rows = _dataController.UpdateEndTime(userID, endTime);
                if (rows == 2)

                {
                    Message = $"退勤時間はすでに登録済みです。";
                    IsClockedOut = true;
                }
                else if (rows > 0)
                {
                    Message = $"退勤時間 {endTimeStr} を登録しました。";
                    IsClockedOut = true;
                }
                else
                {
                    Message = $"退勤時間の登録に失敗もしくは登録済みです。";
                    IsClockedOut = true;
                }
            }
            else
            {
                Message = "時間の形式が正しくありません。";
            }

            //ちょっとここに書くのもなんだけど、どうしたものか(post後のチェックでデータが存在すればreadonlyとする)
            //もっと簡易化出来るはず
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
