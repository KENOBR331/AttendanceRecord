using System;
using System.Data;
using System.Data.SqlClient;

namespace AttendanceRecord.Data
{
    public class DbCon
    {
        private readonly string _connectionString;

        public DbCon(string connectionString)
        {
            _connectionString = connectionString;
        }

        // データを読み取る例（SELECT）
        public DataTable GetAllData()
        {
            var dt = new DataTable();
            //DB接続形式は古いが、1行ずつ読む必要のないくらいのテストプログラムなのでこっちを使用
            //むしろ1行づつ読み込む必要性がない
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                //現状決め打ちだが、後に修正
                string sql = $"SELECT * FROM ";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // データを挿入する例（INSERT）
        public void InsertAttendance(DateTime date, string name)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Attendance (Date, Name) VALUES (@date, @name)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
