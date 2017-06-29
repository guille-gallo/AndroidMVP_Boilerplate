using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WCFAppMetrics.ServiceResponses
{
    public enum StatusResult
    {
        OK = 0,
        ValidationError = 1,
        GeneralError = 2,
        SessionExpired = 3,
				SessionAlive = 4
    }

    [DataContract]
    public class BaseResponse
    {
        BaseStatus _status;

        public BaseResponse()
        {
            _status = new BaseStatus(); 
        }

        [DataMember]
        public BaseStatus status
        {
            get { return _status; }
            set { _status = value; }
        }

        public class BaseStatus
        {
            int _status;
            string _errorMessage;

            public BaseStatus()
            {
                _status = (int)StatusResult.GeneralError;
            }

            public int status
            {
                get { return _status; }
                set { _status = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }
        }
    }
}