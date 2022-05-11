using System;

namespace DB.Client.Core
{
    public class DbCommandException : Exception
    {
        public DbCommandException(string dbError, Exception serializationException, Exception innerException = null)
            : base($"Error: {dbError}", innerException)
        {
            DbError = dbError;
            SerializationException = serializationException;
        }

        public string DbError { get; }
        public Exception SerializationException { get; }
    }
}