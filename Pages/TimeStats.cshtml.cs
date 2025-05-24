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

        // DataController��DI�Ŏ󂯎��R���X�g���N�^
        public TimeStatsModel(DataController dataController)
        {
            _dataController = dataController;
        }

        //�V�X�e���N�̊Y�������X�g�ꗗ
        public List<(int Year, int Month)> MonthList { get; set; } = new();
        //�o�Ύ��ԍ��v
        public double AttendanceTotal { get; set; }
        //���Ύ��ԍ��v
        public double AbsenceTotal { get; set; }

        /// <summary>
        /// �Ǘ��Ґ�p�y�[�W�̂��߁AGet�ŏ����i�y�[�W�J�ڂ��ʓ|�Ƃ����l��URL�����͂��l���j
        /// ���{���͈�ʂł̓A�N�Z�X���ւ��鏈�����{��
        /// </summary>
        /// <param name="year">int �N</param>
        /// <param name="month">int ��</param>
        /// <returns>���ׂẴA�N�V�����̖߂�l</returns>
        public IActionResult OnGet(int? year, int? month)
        {
            if (!year.HasValue || !month.HasValue)
            {
                DateTime now = DateTime.Now;
                return RedirectToPage("TimeStats", new { year = now.Year, month = now.Month });
            }

            // �߂�l���󂯎��
            (List<(int Year, int Month)> MonthList, double AttendanceTotal, double AbsenceTotal) result = _dataController.GetMonthlyStats(year.Value, month.Value);
            MonthList = result.MonthList;
            AttendanceTotal = result.AttendanceTotal;
            AbsenceTotal = result.AbsenceTotal;
            
            

            return Page();
        }
    }
}