using System;
using System.Data.SqlClient;

namespace DBTool.Core
{
    public static class SqlExtensions
    {
        public static T ExecuteDispose<T>(this SqlCommand command, Func<SqlCommand, T> action)
        {
            var result = action(command);
            command.Dispose();
            return result;
        }

        public static int ExecuteNonQuery(this SqlCommand command, string cmdText, bool checkAnyRowAffected = false)
        {
            command.CommandText = cmdText;
            var result = command.ExecuteNonQuery();
            if (checkAnyRowAffected && result < 0)
            {
                throw new DatabaseExecutionException("No rows affected.");
            }
            return result;
        }

        public static int ExecuteNonQueryDispose(this SqlCommand command, string cmdText, bool checkAnyRowAffected = false) => ExecuteDispose(command, c => c.ExecuteNonQuery(cmdText, checkAnyRowAffected));

        public static SqlCommand ExecuteNonQueryOut(this SqlCommand command, string cmdText, out int result, bool checkAnyRowAffected = false) => ExecuteOut(command, c => c.ExecuteNonQuery(cmdText, checkAnyRowAffected), out result);

        public static SqlCommand ExecuteOut<TOut>(this SqlCommand command, Func<SqlCommand, TOut> action, out TOut result)
        {
            result = action(command);
            return command;
        }

        public static SqlDataReader ExecuteReader(this SqlCommand command, string cmdText)
        {
            command.CommandText = cmdText;
            return command.ExecuteReader();
        }

        public static SqlDataReader ExecuteReaderDispose(this SqlCommand command, string cmdText) => ExecuteDispose(command, c => c.ExecuteReader(cmdText));

        public static SqlCommand ExecuteReaderOut(this SqlCommand command, string cmdText, out SqlDataReader result) => ExecuteOut(command, c => c.ExecuteReader(cmdText), out result);

        public static object ExecuteScalar(this SqlCommand command, string cmdText)
        {
            command.CommandText = cmdText;
            return command.ExecuteScalar();
        }

        public static T ExecuteScalar<T>(this SqlCommand command, string cmdText) => (T)ExecuteScalar(command, cmdText);

        public static object ExecuteScalarDispose(this SqlCommand command, string cmdText) => ExecuteDispose(command, c => c.ExecuteScalar(cmdText));

        public static T ExecuteScalarDispose<T>(this SqlCommand command, string cmdText) => ExecuteDispose(command, c => c.ExecuteScalar<T>(cmdText));

        public static SqlCommand ExecuteScalarOut(this SqlCommand command, string cmdText, out object result) => ExecuteOut(command, c => c.ExecuteScalar(cmdText), out result);

        public static SqlCommand ExecuteScalarOut<T>(this SqlCommand command, string cmdText, out T result) => ExecuteOut(command, c => c.ExecuteScalar<T>(cmdText), out result);
    }

    internal static class StringExtensions
    {
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static string FormatWith(this string str, object arg0)
        {
            return string.Format(str, arg0);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}