<<<<<<< HEAD
﻿using System;
using System.Data.SqlClient;
using AttendanceRecord.Models;

namespace AttendanceRecord.Data
{
    public class DbCon
    {
        private readonly string _connectionString;


        public DbCon(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"[DbCon] 接続文字列: {_connectionString}");
        }
        //public DbCon(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}


        /// <summary>
        /// 出勤時間レコードを生成
        /// </summary>
        /// <param name="record">record フォームからPostされたレコードデータ</param>
        public void InsertAttendance(Models.AttendanceRecord record)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                //ひとまず決め打ちでIDは1とする
                string sql = @"INSERT INTO T_kintai (
                                    userid, start_time, end_time, 
                                    rest_start_time, rest_end_time, 
                                    trip_flg, all_day
                               ) VALUES (
                                    @UserId, @StartTime, @EndTime, 
                                    @StartRestTime, @EndRestTime, 
                                    @IsBusinessTrip, @IsTripAll
                               )";



                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    //ユーザIDは現状1で固定化(本来はADなど使用して組織で一意に取得できるキーを入れる)
                    cmd.Parameters.AddWithValue("@UserId", 1);
                    cmd.Parameters.AddWithValue("@StartTime", record.StartTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndTime", record.EndTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartRestTime", record.StartRestTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndRestTime", record.EndRestTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsBusinessTrip", record.IsBusinessTrip);
                    cmd.Parameters.AddWithValue("@IsTripAll", record.IsTripAll);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        #region 出勤時間レコード登録・更新
        /// <summary>
        /// 出勤時間レコード更新(出勤)
        /// </summary>
        /// <param name="userId">int ユーザID</param>
        public int UpdateStartTime(int userId,DateTime startTime)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();


                DateTime dt = DateTime.Now;
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                SqlTransaction transaction = conn.BeginTransaction();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //トランザクション開始
                    cmd.Transaction = transaction;
                    try {
                        // まず UPDATE を試す
                        cmd.CommandText = @"UPDATE T_kintai
                                SET start_time = @StartTime
                                WHERE userid = @UserId AND year = @year AND month = @month AND day = @day ";
                    
                        cmd.Parameters.AddWithValue("@StartTime", startTime); 
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@day", day);


                        //更新量をカウント（0のときにInsertに移行するため）
                        int rowsSql = cmd.ExecuteNonQuery();

                        //Update出来なかった場合はInsertとして処理を行う
                        if (rowsSql == 0)
                        {
                            // INSERT に切り替え
                            cmd.CommandText = @"INSERT INTO T_kintai (userid,year,month,day, start_time)
                                    VALUES (@UserId, @year,@month,@day,@StartTime)";

                            //Updateで使用したパラメータをクリア
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@year", year);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@day", day);
                            cmd.Parameters.AddWithValue("@StartTime", startTime);

                            rowsSql = cmd.ExecuteNonQuery();
                            //コミット
                            transaction.Commit();
                            return rowsSql;
                        }
                    }
                    catch (SqlException ex)
                    {
                        // SQL エラー処理
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        //ロールバック
                        transaction.Rollback();
                    }
                    return 0;
                }
            }
        }
        #endregion

        #region 退勤時間レコード登録・更新
        /// <summary>
        /// 出勤時間レコード更新(退勤)
        /// </summary>
        /// <param name="userId">int ユーザID</param>
        /// <param name="endTime">DateTime 退勤時間</param>
        public int UpdateEndTime(int userId, DateTime endTime)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                DateTime dt = DateTime.Now;
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                SqlTransaction transaction = conn.BeginTransaction();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //トランザクション開始
                    cmd.Transaction = transaction;
                    try
                    {
                        // まず UPDATE を試す
                        cmd.CommandText = @"UPDATE T_kintai
                                SET end_time = @EndTime
                                WHERE userid = @UserId AND year = @year AND month = @month AND day = @day ";


                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@day", day);


                        //更新量をカウント（0のときにInsertに移行するため）
                        int rowsSql = cmd.ExecuteNonQuery();

                        //Update出来なかった場合はInsertとして処理を行う
                        if (rowsSql == 0)
                        {
                            // INSERT に切り替え
                            cmd.CommandText = @"INSERT INTO T_kintai (userid,year,month,day, end_time)
                                    VALUES (@UserId, @year,@month,@day,@end_time)";


                            //Updateで使用したパラメータをクリア
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@year", year);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@day", day);
                            cmd.Parameters.AddWithValue("@end_time", endTime);
                            
                            cmd.ExecuteNonQuery();
                            //コミット
                            transaction.Commit();
                            return rowsSql;
                        }
                    }
                    catch (SqlException ex)
                    {
                        // SQL エラー処理
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        //ロールバック
                        transaction.Rollback();
                    }
                    return 0;
                }
            }
        }
        #endregion
    }
}
=======
﻿using System;
using System.Data.SqlClient;
using AttendanceRecord.Models;

namespace AttendanceRecord.Data
{
    public class DbCon
    {
        private readonly string _connectionString;


        public DbCon(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"[DbCon] 接続文字列: {_connectionString}");
        }
        //public DbCon(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}


        /// <summary>
        /// 出勤時間レコードを生成
        /// </summary>
        /// <param name="record">record フォームからPostされたレコードデータ</param>
        public void InsertAttendance(Models.AttendanceRecord record)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                //ひとまず決め打ちでIDは1とする
                string sql = @"INSERT INTO T_kintai (
                                    userid, start_time, end_time, 
                                    rest_start_time, rest_end_time, 
                                    trip_flg, all_day
                               ) VALUES (
                                    @UserId, @StartTime, @EndTime, 
                                    @StartRestTime, @EndRestTime, 
                                    @IsBusinessTrip, @IsTripAll
                               )";



                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    //ユーザIDは現状1で固定化(本来はADなど使用して組織で一意に取得できるキーを入れる)
                    cmd.Parameters.AddWithValue("@UserId", 1);
                    cmd.Parameters.AddWithValue("@StartTime", record.StartTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndTime", record.EndTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartRestTime", record.StartRestTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndRestTime", record.EndRestTime ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsBusinessTrip", record.IsBusinessTrip);
                    cmd.Parameters.AddWithValue("@IsTripAll", record.IsTripAll);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        #region 出勤時間レコード登録・更新
        /// <summary>
        /// 出勤時間レコード更新(出勤)
        /// </summary>
        /// <param name="userId">int ユーザID</param>
        public int UpdateStartTime(int userId,DateTime startTime)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();


                DateTime dt = DateTime.Now;
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                SqlTransaction transaction = conn.BeginTransaction();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //トランザクション開始
                    cmd.Transaction = transaction;
                    try {
                        // まず UPDATE を試す
                        cmd.CommandText = @"UPDATE T_kintai
                                SET start_time = @StartTime
                                WHERE userid = @UserId AND year = @year AND month = @month AND day = @day ";
                    
                        cmd.Parameters.AddWithValue("@StartTime", startTime); 
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@day", day);


                        //更新量をカウント（0のときにInsertに移行するため）
                        int rowsSql = cmd.ExecuteNonQuery();

                        //Update出来なかった場合はInsertとして処理を行う
                        if (rowsSql == 0)
                        {
                            // INSERT に切り替え
                            cmd.CommandText = @"INSERT INTO T_kintai (userid,year,month,day, start_time)
                                    VALUES (@UserId, @year,@month,@day,@StartTime)";

                            //Updateで使用したパラメータをクリア
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@year", year);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@day", day);
                            cmd.Parameters.AddWithValue("@StartTime", startTime);

                            rowsSql = cmd.ExecuteNonQuery();
                            //コミット
                            transaction.Commit();
                            return rowsSql;
                        }
                    }
                    catch (SqlException ex)
                    {
                        // SQL エラー処理
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        //ロールバック
                        transaction.Rollback();
                    }
                    return 0;
                }
            }
        }
        #endregion

        #region 退勤時間レコード登録・更新
        /// <summary>
        /// 出勤時間レコード更新(退勤)
        /// </summary>
        /// <param name="userId">int ユーザID</param>
        /// <param name="endTime">DateTime 退勤時間</param>
        public int UpdateEndTime(int userId, DateTime endTime)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                DateTime dt = DateTime.Now;
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                SqlTransaction transaction = conn.BeginTransaction();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //トランザクション開始
                    cmd.Transaction = transaction;
                    try
                    {
                        // まず UPDATE を試す
                        cmd.CommandText = @"UPDATE T_kintai
                                SET end_time = @EndTime
                                WHERE userid = @UserId AND year = @year AND month = @month AND day = @day ";


                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@day", day);


                        //更新量をカウント（0のときにInsertに移行するため）
                        int rowsSql = cmd.ExecuteNonQuery();

                        //Update出来なかった場合はInsertとして処理を行う
                        if (rowsSql == 0)
                        {
                            // INSERT に切り替え
                            cmd.CommandText = @"INSERT INTO T_kintai (userid,year,month,day, end_time)
                                    VALUES (@UserId, @year,@month,@day,@end_time)";


                            //Updateで使用したパラメータをクリア
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@year", year);
                            cmd.Parameters.AddWithValue("@month", month);
                            cmd.Parameters.AddWithValue("@day", day);
                            cmd.Parameters.AddWithValue("@end_time", endTime);
                            
                            cmd.ExecuteNonQuery();
                            //コミット
                            transaction.Commit();
                            return rowsSql;
                        }
                    }
                    catch (SqlException ex)
                    {
                        // SQL エラー処理
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        //ロールバック
                        transaction.Rollback();
                    }
                    return 0;
                }
            }
        }
        #endregion
    }
}
>>>>>>> 525647e00824706edcbd2624343b3a74fe2ae07b
