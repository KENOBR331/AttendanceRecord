using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceRecord.Data;
using Microsoft.Extensions.Configuration;
using AttendanceRecord.Models;

namespace AttendanceRecord.Pages
{
    public class TimeInputModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly DbCon _db;
        [BindProperty]
        public string StartTime { get; set; }

        [BindProperty]
        public string EndTime { get; set; }

        public string Message { get; set; }
        public string CurrentTime { get; private set; }
        public bool IsClockedIn { get; set; }
        public bool IsClockedOut { get; set; }
        
        public TimeInputModel(DbCon dbCon)
        {
            _db = dbCon ?? throw new ArgumentNullException(nameof(dbCon));  // null�`�F�b�N
        }
        public void OnGet()
        {
            //ClockInTime = DateTime.Now;
            //CurrentTime = DateTime.Now.ToString("HH:mm");
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        public IActionResult OnPost()
        {
            //0�F�o�^���s�A1�F�o�^�i�X�V�j����
            int dbStat = 0;
            var action = Request.Form["action"];
            var startTime = Request.Form["startTime"];
            var endTime = Request.Form["endTime"];
            var record = new Models.AttendanceRecord
            {
                StartTime = DateTime.TryParse(Request.Form["startTime"], out var sTime) ? sTime : (DateTime?)null,
                EndTime = DateTime.TryParse(Request.Form["endTime"], out var eTime) ? eTime : (DateTime?)null,
                StartRestTime = DateTime.TryParse(Request.Form["startRestTime"], out var srTime) ? srTime : (DateTime?)null,
                EndRestTime = DateTime.TryParse(Request.Form["endRestTime"], out var erTime) ? erTime : (DateTime?)null,
                IsBusinessTrip = Request.Form["isBusinessTrip"] == "on",
                IsTripAll = Request.Form["isTripAll"] == "on"
            };

            if (!string.IsNullOrEmpty(startTime))
            {
                HttpContext.Session.SetString("StartTime", startTime);
            }

            HttpContext.Session.SetString("StartTime", startTime);
            HttpContext.Session.SetString("EndTime", endTime);

            if (action == "clockin")
            {

                //�ЂƂ܂����[�UID��1�ŌŒ�
                dbStat = _db.UpdateStartTime(1, DateTime.Parse(startTime));
                if (dbStat == 1)
                {
                    Message = $"�o�Ύ��� {startTime} ��o�^���܂����B";
                }
                else
                {
                    Message = $"�o�Ύ��Ԃ̓o�^�Ɏ��s�������͓o�^�ς݂ł��B";
                }
                //if (DateTime.TryParse(Request.Form["startTime"], out var sTime)) { 
                

                IsClockedIn = true;
                //}
            }
            else if (action == "clockout")
            {
                dbStat = _db.UpdateEndTime(1, DateTime.Parse(endTime));
                if (dbStat == 1)
                {
                    Message = $"�ދΎ��� {endTime} ��o�^���܂����B";
                }
                else
                {
                    Message = $"�ދΎ��Ԃ̓o�^�Ɏ��s�������͓o�^�ς݂ł��B";
                }

                IsClockedOut = true;
            }
            CurrentTime = DateTime.Now.ToString("HH:mm");
            //return RedirectToPage();
            return Page();

        }
    }

}
