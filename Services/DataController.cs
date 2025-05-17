using AttendanceRecord.Data;
using AttendanceRecord.Models;
using System;
using System.Data.SqlClient;

namespace AttendanceRecord.Services
{
    public class DataController
    {
        private readonly DbConnect _dbConnect;

        public DataController(DbConnect dbConnect)
        {
            _dbConnect = dbConnect;
        }
        public int UpdateStartTime(int userId, DateTime startTime)
        {
            using var conn = _dbConnect.GetConnection();
            conn.Open();
            var tran = conn.BeginTransaction();

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            try
            {
                var now = DateTime.Now;

                cmd.CommandText = @"UPDATE T_kintai
                                    SET start_time = @StartTime
                                    WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day";

                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Year", now.Year);
                cmd.Parameters.AddWithValue("@Month", now.Month);
                cmd.Parameters.AddWithValue("@Day", now.Day);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    cmd.CommandText = @"INSERT INTO T_kintai (userid, year, month, day, start_time)
                                        VALUES (@UserId, @Year, @Month, @Day, @StartTime)";
                    rows = cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return rows;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine($"StartTime Error: {ex.Message}");
                return 0;
            }
        }

        public int UpdateEndTime(int userId, DateTime endTime)
        {
            using var conn = _dbConnect.GetConnection();
            conn.Open(); 
            var tran = conn.BeginTransaction();

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            try
            {
                var now = DateTime.Now;

                cmd.CommandText = @"UPDATE T_kintai
                            SET end_time = @EndTime
                            WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day";

                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Year", now.Year);
                cmd.Parameters.AddWithValue("@Month", now.Month);
                cmd.Parameters.AddWithValue("@Day", now.Day);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    cmd.CommandText = @"INSERT INTO T_kintai (userid, year, month, day, end_time)
                                VALUES (@UserId, @Year, @Month, @Day, @EndTime)";
                    rows = cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return rows;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine($"EndTime Error: {ex.Message}");
                return 0;
            }
        }

        public (List<(int Year, int Month)> MonthList, double AttendanceTotal, double AbsenceTotal) GetMonthlyStats(int year, int month)
        {
            var monthList = new List<(int Year, int Month)>();
            double attendanceTotal = 0;
            double absenceTotal = 0;

            using var conn = _dbConnect.GetConnection();
            conn.Open();

            // 月リストの取得
            using (var monthCmd = new SqlCommand(@"
      SELECT DISTINCT 
            year, 
           RIGHT('00' + CAST(CAST(LTRIM(RTRIM(month)) AS INT) AS VARCHAR(2)), 2) AS month
        FROM T_Kintai
        WHERE del_flg = 0 AND month IS NOT NULL
        ORDER BY year DESC, RIGHT('00' + CAST(CAST(LTRIM(RTRIM(month)) AS INT) AS VARCHAR(2)), 2)  DESC;", conn))
                    {
                using (var reader = monthCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int y = Convert.ToInt32(reader["year"]);
                        int m = Convert.ToInt32(reader["month"]);
                        monthList.Add((y, m));
                    }
                }
            }

            // 勤務時間集計
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

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@month", month);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        attendanceTotal = reader.IsDBNull(0) ? 0 : reader.GetInt32(0) / 60.0;
                        absenceTotal = reader.IsDBNull(1) ? 0 : reader.GetInt32(1) / 60.0;
                    }
                }
            }

            return (monthList, attendanceTotal, absenceTotal);
        }


    }
}
