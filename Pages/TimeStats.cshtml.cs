using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using AttendanceRecord.Services;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;

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

        //システム年の該当月リスト一覧
        public List<(int Year, int Month)> MonthList { get; set; } = new();
        //出勤時間合計
        public double AttendanceTotal { get; set; }
        //欠勤時間合計
        public double AbsenceTotal { get; set; }

        /// <summary>
        /// 管理者専用ページのため、Getで処理（ページ遷移が面倒という人はURL直入力も考慮）
        /// ※本来は一般ではアクセスを禁ずる処理を施す
        /// </summary>
        /// <param name="year">int 年</param>
        /// <param name="month">int 月</param>
        /// <returns>すべてのアクションの戻り値</returns>
        public IActionResult OnGet(int? year, int? month)
        {
            if (!year.HasValue || !month.HasValue)
            {
                DateTime now = DateTime.Now;
                return RedirectToPage("TimeStats", new { year = now.Year, month = now.Month });
            }

            // 戻り値を受け取る
            (List<(int Year, int Month)> MonthList, double AttendanceTotal, double AbsenceTotal) result = _dataController.GetMonthlyStats(year.Value, month.Value);
            MonthList = result.MonthList;
            AttendanceTotal = result.AttendanceTotal;
            AbsenceTotal = result.AbsenceTotal;
            
            

            return Page();
        }
    }
}