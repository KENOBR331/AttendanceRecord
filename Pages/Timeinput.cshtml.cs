using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AttendanceRecord.Pages
{
    public class TimeInputModel : PageModel
    {
        [BindProperty]
        public string StartTime { get; set; }

        [BindProperty]
        public string EndTime { get; set; }

        public string Message { get; set; }
        public string CurrentTime { get; private set; }
        public bool IsClockedIn { get; set; }

        public void OnGet()
        {
            //ClockInTime = DateTime.Now;
            //CurrentTime = DateTime.Now.ToString("HH:mm");
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        public IActionResult OnPost()
        {
            
            var action = Request.Form["action"];
            var startTime = Request.Form["startTime"];
            var endTime = Request.Form["endTime"];

            HttpContext.Session.SetString("StartTime", startTime);
            HttpContext.Session.SetString("EndTime", endTime);

            if (action == "clockin")
            {
                Message = $"èoãŒéûä‘ {startTime} Çìoò^ÇµÇ‹ÇµÇΩÅB";
                IsClockedIn = true;
            }
            else if (action == "clockout")
            {
                Message = $"ëﬁãŒéûä‘ {endTime} Çìoò^ÇµÇ‹ÇµÇΩÅB";
            }

            return Page();
        }
    }

}
