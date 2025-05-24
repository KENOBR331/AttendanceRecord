//最近Tabでコメントすら補完してくれるのがありがたいのか、練習にもならん・・・
//だいぶWebアプリ思い出してきた
//ゲーム用にコンソールアプリばっかり作ってたのがつらい
using AttendanceRecord.Data;
using AttendanceRecord.Models;
//using Microsoft.Data.SqlClient;//使わないや
using System;
using System.Data;
using System.Data.SqlClient;


namespace AttendanceRecord.Services
{
    /// <summary>
    /// 各種データ操作
    /// </summary>
    public class DataController
    {
        private readonly DbConnect _dbConnect;

        public DataController(DbConnect dbConnect)
        {
            _dbConnect = dbConnect;
        }

        /// <summary>
        /// 勤務時間が既に登録されている場合は初期表示時に取得する
        /// </summary>
        /// <returns></returns>
        public DataTable initTimeInput(int userId)
        {
            //コネクション取得
            using SqlConnection conn = _dbConnect.GetConnection();
            //ここでコネクションを開く
            conn.Open();
            DateTime now = DateTime.Now;
            //コマンド用オブジェクト作成
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT start_time,end_time FROM T_kintai WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day ";
            DataTable dt = new DataTable();
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Year", now.Year);
            cmd.Parameters.AddWithValue("@Month", now.Month.ToString().PadLeft(2, '0'));
            cmd.Parameters.AddWithValue("@Day", now.Day.ToString().PadLeft(2, '0'));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 出勤時間を更新
        /// </summary>
        /// <param name="userId">ユーザID(1固定)</param>
        /// <param name="startTime">勤務開始時間</param>
        /// <returns>
        /// 0:エラー 
        /// 1:更新または新規登録（更新はなくす予定）
        /// 2:既に登録済み
        /// </returns>
        [Obsolete]
         public int UpdateStartTime(int userId, DateTime startTime)
        {
            //コネクション取得
            using SqlConnection conn = _dbConnect.GetConnection();
            //ここでコネクションを開く
            conn.Open();
            //トランザクション開始
            SqlTransaction tran = conn.BeginTransaction();

            //コマンド用オブジェクト作成
            using SqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            DateTime now = DateTime.Now;
            //※ここでSELECTを実行し、登録済みか否かを判別する。
            
            cmd.CommandText = @"SELECT COUNT(start_time) AS RESULT FROM T_kintai
                                    WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day";


            DataTable dt = new DataTable();
            //SqlCommand sqlcmd = new SqlCommand(cmd.CommandText, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Year", now.Year);
            cmd.Parameters.AddWithValue("@Month",  now.Month.ToString().PadLeft(2,'0'));
            cmd.Parameters.AddWithValue("@Day", now.Day.ToString().PadLeft(2, '0'));

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //SELECTの実行結果をDataTableに格納
            adapter.Fill(dt);
            //int型にキャストして、結果を取得する
            if ((int)dt.Rows[0]["RESULT"] > 0)
            {
                return 2;//すでにデータが登録済みの場合は2を返す(登録済みと表示するため)
            }
            int rows = 0;
            //念の為コマンドパラメタクリア
            cmd.Parameters.Clear();
            try
            {
                
                cmd.CommandText = @"UPDATE T_kintai
                                    SET start_time = @StartTime
                                    WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day";

                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Year", now.Year);
                cmd.Parameters.AddWithValue("@Month", now.Month.ToString().PadLeft(2, '0'));
                cmd.Parameters.AddWithValue("@Day", now.Day.ToString().PadLeft(2, '0'));

                rows = cmd.ExecuteNonQuery();
                //ここではコマンドパラメタをクリアしない
                //（UPDATEで使われているパラメタをINSERTに流用）
                if (rows == 0) { 
                    
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

        /// <summary>
        /// 退勤時間を登録
        /// </summary>
        /// <param name="userId">ユーザID</param>
        /// <param name="endTime">退勤時間</param>
        /// <returns>
        /// 0:エラー 
        /// 1:更新または新規登録（更新はなくす予定）
        /// 2:既に登録済み
        /// </returns>
        public int UpdateEndTime(int userId, DateTime endTime)
        {
            using SqlConnection conn = _dbConnect.GetConnection();
            conn.Open(); 
            SqlTransaction tran = conn.BeginTransaction();

            DateTime now = DateTime.Now;
            //コマンド用オブジェクト作成
            using SqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            //※ここでSELECTを実行し、登録済みか否かを判別する。

            cmd.CommandText = @"SELECT COUNT(end_time) AS RESULT FROM T_kintai
                                    WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day";


            DataTable dt = new DataTable();
            //SqlCommand sqlcmd = new SqlCommand(cmd.CommandText, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Year", now.Year);
            cmd.Parameters.AddWithValue("@Month", now.Month.ToString().PadLeft(2, '0'));
            cmd.Parameters.AddWithValue("@Day", now.Day.ToString().PadLeft(2, '0'));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //SELECTの実行結果をDataTableに格納
            adapter.Fill(dt);
            //int型にキャストして、結果を取得する
            if ((int)dt.Rows[0]["RESULT"] > 0)
            {
                return 2;//すでにデータが登録済みの場合は2を返す(登録済みと表示するため)
            }
            cmd.Transaction = tran;

            cmd.Parameters.Clear();
            try
            {
                cmd.CommandText = @"UPDATE T_kintai
                            SET end_time = @EndTime
                            WHERE userid = @UserId AND year = @Year AND month = @Month AND day = @Day";

                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Year", now.Year);
                cmd.Parameters.AddWithValue("@Month", now.Month.ToString().PadLeft(2, '0'));
                cmd.Parameters.AddWithValue("@Day", now.Day.ToString().PadLeft(2, '0'));

                int rows = cmd.ExecuteNonQuery();

                //ここではコマンドパラメタをクリアしない
                //（UPDATEで使われているパラメタをINSERTに流用）
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

        /// <summary>
        /// 月ごとの勤務時間を取得
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns>月ごとの合計勤務、欠勤時間
        /// yearを入れているのはその年の月を絞るため</returns>
        public (List<(int Year, int Month)> MonthList, double AttendanceTotal, double AbsenceTotal) GetMonthlyStats(int year, int month)
        {
            List<(int Year, int Month)> monthList = new List<(int Year, int Month)>();
            double attendanceTotal = 0;
            double absenceTotal = 0;

            using SqlConnection conn = _dbConnect.GetConnection();
            conn.Open();

            // 月リストの取得
            using (SqlCommand monthCmd = new SqlCommand(@"
              SELECT DISTINCT 
                    year, 
                   RIGHT('00' + CAST(CAST(LTRIM(RTRIM(month)) AS INT) AS VARCHAR(2)), 2) AS month
                FROM T_Kintai
                WHERE del_flg = 0 AND month IS NOT NULL
                ORDER BY year DESC, RIGHT('00' + CAST(CAST(LTRIM(RTRIM(month)) AS INT) AS VARCHAR(2)), 2)  DESC;", conn))
                    {
                using (SqlDataReader reader = monthCmd.ExecuteReader())
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

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@month", month);

                using (SqlDataReader reader = cmd.ExecuteReader())
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
