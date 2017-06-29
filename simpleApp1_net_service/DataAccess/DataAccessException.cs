using System;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace AppMetrics.Data.ADONETDataAccess
{
    [Serializable()]
    public class DataAccessException : Exception
    {
        private int _errInterno;
        private string _message;

        public int errInterno
        {
            get { return _errInterno; }
        }

        public override string Message
        {
            get { return _message; }
        }

        public DataAccessException(DataAccessException ex)
            : base(ex.InnerException.Message, ex)
        {
            CargarErrorInterno(ex);
        }

        public DataAccessException(string message, Exception ex)
            : base(message, ex)
        {
            CargarErrorInterno(ex);
        }

        public DataAccessException(string message, Exception ex, int errInterno)
            : base(message, ex)
        {
            _errInterno = errInterno;
            CargarErrorInterno(ex);
        }


        public DataAccessException(string message, int errInterno)
            : base(message)
        {
            _errInterno = errInterno;
            _message = message;
        }

        protected DataAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        private void CargarErrorInterno(Exception ex)
        {
            switch (ex.Source)
            {
                case ".Net SqlClient Data Provider":
                    SqlException sqlEx = (SqlException)ex.GetBaseException();

                    switch (sqlEx.Number)
                    {
                        case -2:
                            _errInterno = -2147217871;
                            break;

                        default:
                            _errInterno = sqlEx.Number;
                            break;
                    }

                    _message = sqlEx.Message;
                    break;
                default:
                    _message = ex.Message;
                    _errInterno = HResult;
                    break;
            }
        }

    }
}