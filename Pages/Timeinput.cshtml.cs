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
            _db = dbCon ?? throw new ArgumentNullException(nameof(dbCon));  // nullÉ`ÉFÉbÉN
        }
        public void OnGet()
        {
            //ClockInTime = DateTime.Now;
            //CurrentTime = DateTime.Now.ToString("HH:mm");
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        public IActionResult OnPost()
        {
            //0ÅFìoò^é∏îsÅA1ÅFìoò^ÅiçXêVÅjê¨å˜
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

                //Ç–Ç∆Ç‹Ç∏ÉÜÅ[ÉUIDÇÕ1Ç≈å≈íË
                dbStat = _db.UpdateStartTime(1, DateTime.Parse(startTime));
                if (dbStat == 1)
                {
                    Message = $"èoãŒéûä‘ {startTime} Çìoò^ÇµÇ‹ÇµÇΩÅB";
                }
                else
                {
                    Message = $"èoãŒéûä‘ÇÃìoò^Ç…é∏îsÇ‡ÇµÇ≠ÇÕìoò^çœÇ›Ç≈Ç∑ÅB";
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
                    Message = $"ëﬁãŒéûä‘ {endTime} Çìoò^ÇµÇ‹ÇµÇΩÅB";
                }
                else
                {
                    Message = $"ëﬁãŒéûä‘ÇÃìoò^Ç…é∏îsÇ‡ÇµÇ≠ÇÕìoò^çœÇ›Ç≈Ç∑ÅB";
                }

                IsClockedOut = true;
            }
            CurrentTime = DateTime.Now.ToString("HH:mm");
            //return RedirectToPage();
            return Page();

        }
    }

}
