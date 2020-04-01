using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace REST_test
{
    static class DBUtils
    {
        private static string Host { get; } = "127.0.0.1";
        private static string Database { get; } = "autotests";
        private static string Port { get; } = "3306";
        private static string Username { get; } = "root";
        private static string Password { get; } = "кщще";

        public static MySqlConnection DBConnection()
        {
            var concat = "Server=" + Host + ";Database=" + Database + ";Port="
                         + Port + ";User Id=" + Username + ";Password=" + Password;

            var myConnection = new MySqlConnection(concat);
            return myConnection;
        }
    }
}
