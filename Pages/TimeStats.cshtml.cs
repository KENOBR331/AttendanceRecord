using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AttendanceRecord.Pages
{
    public class TimeStatsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public TimeStatsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int AttendanceCount { get; set; }
        public int AbsenceCount { get; set; }
        public List<int> UserIds { get; set; } = new();
        public List<double> WorkHours { get; set; } = new();
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

            LoadData(year.Value, month.Value);
            return Page();
        }

        private void LoadData(int year, int month)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // ▼ 月リストの取得
                using (var monthCmd = new SqlCommand(@"
                    SELECT DISTINCT year, month 
                    FROM T_Kintai 
                    WHERE del_flg = 0 
                    ORDER BY year DESC, month DESC", connection))
                {
                    using (var reader = monthCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int y = int.Parse(reader["year"].ToString());
                            int m = int.Parse(reader["month"].ToString());
                            MonthList.Add((y, m));
                        }
                    }
                }

                // ▼ 勤務時間集計
                string sql = @"
                    SELECT 
                        SUM(CASE 
                            WHEN start_time IS NOT NULL AND end_time IS NOT NULL 
                            THEN DATEDIFF(MINUTE, start_time, end_time)  - ISNULL(DATEDIFF(MINUTE, rest_start_time, rest_end_time), 0)
                            ELSE 0
                        END) AS AttendanceTotalMinutes,

                        SUM(CASE 
                            WHEN start_time IS NULL OR end_time IS NULL 
                            THEN 8 * 60
                            ELSE 0
                        END) AS AbsenceTotalMinutes
                    FROM T_Kintai
                    WHERE del_flg = 0 AND year = @year AND month = @month";

                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@month", month);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AttendanceTotal = reader.IsDBNull(0) ? 0 : reader.GetInt32(0) / 60.0;
                            AbsenceTotal = reader.IsDBNull(1) ? 0 : reader.GetInt32(1) / 60.0;
                        }
                    }
                }

            }
        }
    }
}
