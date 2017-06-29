using System;
using System.Runtime.Serialization;

namespace AppMetrics.Common
{
    [Serializable()]
    public class ValidationException : Exception
    {
        private string _message = "";

        public override string Message
        {
            get { return _message; }
        }

        public ValidationException(string message)
            : base(message)
        {
            _message = message;
        }
    }
}
