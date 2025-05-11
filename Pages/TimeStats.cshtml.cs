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
            string whereClause = "WHERE del_flg = 0 AND year = @year AND month = @month";

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
                using (var cmd = new SqlCommand($@"
                    SELECT 
                        userid, 
                        DATEDIFF(MINUTE, start_time, end_time) - DATEDIFF(MINUTE, rest_start_time, rest_end_time) AS WorkMinutes
                    FROM T_Kintai
                    {whereClause}", connection))
                {
                    cmd.Parameters.AddWithValue("@year", year.ToString("0000"));
                    cmd.Parameters.AddWithValue("@month", month.ToString("00"));

                    using (var reader = cmd.ExecuteReader())
                    {
                        double attendanceTotal = 0;
                        double absenceTotal = 0;

                        while (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            int workMinutes = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                            UserIds.Add(userId);
                            WorkHours.Add(workMinutes / 60.0);

                            attendanceTotal += workMinutes;
                            if (workMinutes == 0) absenceTotal += 8 * 60;
                        }

                        AttendanceTotal = attendanceTotal / 60.0;
                        AbsenceTotal = absenceTotal / 60.0;
                    }
                }
            }
        }
    }
}
