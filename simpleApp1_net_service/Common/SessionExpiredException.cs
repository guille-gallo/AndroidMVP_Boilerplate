using System;
using System.Runtime.Serialization;

namespace AppMetrics.Common
{
    [Serializable()]
    public class SessionExpiredException : Exception
    {
        private string _message = "";

        public override string Message
        {
            get { return _message; }
        }

        public SessionExpiredException(string message)
            : base(message)
        {
            _message = message;
        }
    }
}
