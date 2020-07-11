namespace DataTableCreateLibrary
{
    using System;
    using System.Runtime.Serialization;

    public class UserException : ApplicationException
    {
        public UserException()
        {
        }

        public UserException(string message) : base(message)
        {
        }

        public UserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
