﻿using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AttendanceRecord.Data
{
    public class DbConnect
    {
        private readonly string _connectionString;

        public DbConnect(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is not configured.");
        }

        /// <summary>
        /// SQL Serverへの接続を取得するメソッド(newここでいいんだっけか？)
        /// </summary>
        /// <returns>コネクションを返す</returns>
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString); // Openは呼ばずに返す
        }
    }
}
