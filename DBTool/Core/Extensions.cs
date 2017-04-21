using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTool
{
    public static class SqlExtensions
    {
        public static SqlCommand Script(this SqlCommand cmd, string script)
        {
            cmd.CommandText = script;
            return cmd;
        }
        public static SqlCommand Open(this SqlCommand cmd)
        {
            cmd.Connection.Open();
            return cmd;
        }
        public static SqlCommand Close(this SqlCommand cmd)
        {
            cmd.Connection.Close();
            return cmd;
        }
        public static object ExecuteScalar(this SqlCommand cmd, bool close)
        {
            var r = cmd.ExecuteScalar();
            if(close) cmd.Close();
            return r;
        }
        public static SqlDataReader ExecuteReader(this SqlCommand cmd, bool close)
        {
            var r = cmd.ExecuteReader();
            if (close) cmd.Close();
            return r;
        }
        public static int ExecuteNonQuery(this SqlCommand cmd, bool close)
        {
            var r = cmd.ExecuteNonQuery();
            if (close) cmd.Close();
            return r;
        }



        public static object ExecuteQuery(this SqlCommand cmd, string script, bool close = false)
        {
            return cmd.Script(script).ExecuteScalar(close);
        }
        public static SqlDataReader ExecuteReaderQuery(this SqlCommand cmd, string script, bool close = false)
        {
            return cmd.Script(script).ExecuteReader(close);
        }
        public static int ExecuteScript(this SqlCommand cmd, string script, bool close = false)
        {
            return cmd.Script(script).ExecuteNonQuery(close);
        }
    }
    static class StringExtensions
    {
        public static string Form(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
        public static string Form(this string str, object arg0)
        {
            return string.Format(str, arg0);
        }
    }
}
