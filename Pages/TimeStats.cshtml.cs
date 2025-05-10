using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AttendanceRecord.Pages
{
    public class TimeStatsModel : PageModel
{
    public int AttendanceCount { get; set; }
    public int AbsenceCount { get; set; }
    public List<int> UserIds { get; set; } = new();
    public List<double> WorkHours { get; set; } = new();

    public List<(int Year, int Month)> MonthList { get; set; } = new();

    // 追加: 勤務合計時間と欠勤合計時間
    public double AttendanceTotal { get; set; }
    public double AbsenceTotal { get; set; }

    public void OnGet()
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=kintai;User Id=sa;Password=Ken;";

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // 月一覧の取得
            string query = "SELECT DISTINCT year, month FROM T_Kintai WHERE del_flg = 0 ORDER BY year, month;";
            using (var cmd = new SqlCommand(query, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int year = int.Parse(reader.GetString(0).Trim());
                    int month = int.Parse(reader.GetString(1).Trim());
                    MonthList.Add((year, month));
                }
            }
        }

        // 出勤と欠勤の集計
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // 出勤と欠勤の集計
            using (var cmd = new SqlCommand(@"
                SELECT 
                    CASE 
                        WHEN start_time IS NOT NULL AND end_time IS NOT NULL THEN '出勤'
                        ELSE '欠勤'
                    END AS Status,
                    COUNT(*) 
                FROM T_Kintai
                WHERE del_flg = 0
                GROUP BY 
                    CASE 
                        WHEN start_time IS NOT NULL AND end_time IS NOT NULL THEN '出勤'
                        ELSE '欠勤'
                    END", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string status = reader.GetString(0);  // '出勤' または '欠勤'
                    int count = reader.GetInt32(1);       // 出勤・欠勤の人数

                    if (status == "出勤") AttendanceCount = count;
                    else if (status == "欠勤") AbsenceCount = count;
                }
            }

            // 勤務時間の合計と欠勤時間の合計の集計
            double attendanceTotal = 0;
            double absenceTotal = 0;

            using (var cmd = new SqlCommand(@"
                SELECT 
                    userid, 
                    DATEDIFF(MINUTE, start_time, end_time) - DATEDIFF(MINUTE, rest_start_time, rest_end_time) AS WorkMinutes
                FROM T_Kintai
                WHERE del_flg = 0", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int userId = reader.GetInt32(0);        // ユーザーID
                    int workMinutes = 0;  // デフォルト値を設定
                    if (!reader.IsDBNull(1))
                    {
                        workMinutes = reader.GetInt32(1);
                    }
                    else
                    {
                        // NULL の場合の処理（例：デフォルト値設定）
                        workMinutes = 0;  // もしくは別の適切な処理
                    }
                    UserIds.Add(userId);
                    WorkHours.Add(workMinutes / 60.0);  // 時間単位に変換

                    // 勤務合計時間の計算（出勤時間を足す）
                    attendanceTotal += workMinutes;

                    // 欠勤時間の計算（仮定：勤務時間が0であれば欠勤）
                    if (workMinutes == 0)
                    {
                        absenceTotal += 8 * 60;  // 8時間分の欠勤として仮定
                    }
                }
            }

            // 計算結果をモデルにセット
            AttendanceTotal = attendanceTotal / 60.0;  // 時間単位に変換
            AbsenceTotal = absenceTotal / 60.0;  // 時間単位に変換
        }
    }
}
}