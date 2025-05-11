using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration; // ← これを追加
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

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

        /// <summary>
        /// 勤務時間の集計
        /// </summary>
        /// <param name="year">int 年</param>
        /// <param name="month">int 月</param>
        public void OnGet(int? year, int? month)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string whereClause = "WHERE del_flg = 0";
            if (year.HasValue && month.HasValue)
            {
                whereClause += " AND year = @year AND month = @month";
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ▼ 年月のリストを取得
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
                

                using (var cmd = new SqlCommand($@"
            SELECT 
                userid, 
                DATEDIFF(MINUTE, start_time, end_time) - DATEDIFF(MINUTE, rest_start_time, rest_end_time) AS WorkMinutes
            FROM T_Kintai
            {whereClause}", connection))
                {
                    if (year.HasValue && month.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@year", year.Value.ToString("0000"));
                        cmd.Parameters.AddWithValue("@month", month.Value.ToString("00"));
                    }

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