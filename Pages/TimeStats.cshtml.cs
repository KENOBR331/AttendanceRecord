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

    // �ǉ�: �Ζ����v���Ԃƌ��΍��v����
    public double AttendanceTotal { get; set; }
    public double AbsenceTotal { get; set; }

    public void OnGet()
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=kintai;User Id=sa;Password=Ken;";

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // ���ꗗ�̎擾
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

        // �o�΂ƌ��΂̏W�v
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // �o�΂ƌ��΂̏W�v
            using (var cmd = new SqlCommand(@"
                SELECT 
                    CASE 
                        WHEN start_time IS NOT NULL AND end_time IS NOT NULL THEN '�o��'
                        ELSE '����'
                    END AS Status,
                    COUNT(*) 
                FROM T_Kintai
                WHERE del_flg = 0
                GROUP BY 
                    CASE 
                        WHEN start_time IS NOT NULL AND end_time IS NOT NULL THEN '�o��'
                        ELSE '����'
                    END", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string status = reader.GetString(0);  // '�o��' �܂��� '����'
                    int count = reader.GetInt32(1);       // �o�΁E���΂̐l��

                    if (status == "�o��") AttendanceCount = count;
                    else if (status == "����") AbsenceCount = count;
                }
            }

            // �Ζ����Ԃ̍��v�ƌ��Ύ��Ԃ̍��v�̏W�v
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
                    int userId = reader.GetInt32(0);        // ���[�U�[ID
                    int workMinutes = 0;  // �f�t�H���g�l��ݒ�
                    if (!reader.IsDBNull(1))
                    {
                        workMinutes = reader.GetInt32(1);
                    }
                    else
                    {
                        // NULL �̏ꍇ�̏����i��F�f�t�H���g�l�ݒ�j
                        workMinutes = 0;  // �������͕ʂ̓K�؂ȏ���
                    }
                    UserIds.Add(userId);
                    WorkHours.Add(workMinutes / 60.0);  // ���ԒP�ʂɕϊ�

                    // �Ζ����v���Ԃ̌v�Z�i�o�Ύ��Ԃ𑫂��j
                    attendanceTotal += workMinutes;

                    // ���Ύ��Ԃ̌v�Z�i����F�Ζ����Ԃ�0�ł���Ό��΁j
                    if (workMinutes == 0)
                    {
                        absenceTotal += 8 * 60;  // 8���ԕ��̌��΂Ƃ��ĉ���
                    }
                }
            }

            // �v�Z���ʂ����f���ɃZ�b�g
            AttendanceTotal = attendanceTotal / 60.0;  // ���ԒP�ʂɕϊ�
            AbsenceTotal = absenceTotal / 60.0;  // ���ԒP�ʂɕϊ�
        }
    }
}
}