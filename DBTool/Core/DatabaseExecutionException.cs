using System;
using System.Runtime.Serialization;

namespace DBTool.Core
{
    public class DatabaseExecutionException : Exception
    {
        public DatabaseExecutionException()
        {
        }

        public DatabaseExecutionException(string message) : base(message)
        {
        }

        public DatabaseExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}