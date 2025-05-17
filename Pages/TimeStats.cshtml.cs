using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using AttendanceRecord.Services;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AttendanceRecord.Pages
{
    public class TimeStatsModel : PageModel
    {
        private readonly DataController _dataController;

        // DataControllerをDIで受け取るコンストラクタ
        public TimeStatsModel(DataController dataController)
        {
            _dataController = dataController;
        }

        public List<(int Year, int Month)> MonthList { get; set; } = new();
        public double AttendanceTotal { get; set; }
        public double AbsenceTotal { get; set; }

        public IActionResult OnGet(int? year, int? month)
        {
            if (!year.HasValue || !month.HasValue)
            {
                var now = DateTime.Now;
                return RedirectToPage("TimeStats", new { year = now.Year, month = now.Month });
            }

            // 戻り値を受け取る
            var result = _dataController.GetMonthlyStats(year.Value, month.Value);
            MonthList = result.MonthList;
            AttendanceTotal = result.AttendanceTotal;
            AbsenceTotal = result.AbsenceTotal;

            return Page();
        }
    }
}