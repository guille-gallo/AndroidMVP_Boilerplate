using System;
using System.Runtime.Serialization;
using AppMetrics.Logic.BaseClass;

namespace AppMetrics.Logic
{
    /// <summary>
    /// Clase de Excepciones del Core
    /// </summary>
    [Serializable()]
    public class CoreException : Exception
    {
        private long _errInterno = 0;
        private string _message = "";

        /// <summary>
        /// Error Interno
        /// </summary>
        public long errInterno
        {
            get { return _errInterno; }
        }

        /// <summary>
        /// Mensaje interno del Error
        /// </summary>
        public override string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        /// <param name="ex">Excepcion de tipo CoreException</param>
        public CoreException(CoreException ex)
            : base(ex.InnerException.Message, ex)
        {
            _message = ex.Message;
            _errInterno = ex.errInterno;

        }

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        /// <param name="message">Mensaje interno.</param>
        /// <param name="ex">Excepcion de tipo Generico.</param>
        public CoreException(string message, Exception ex)
            : base(message, ex)
        {
            _message = message;
            CargarErrorInterno(ex);
        }

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        /// <param name="message">Mensaje interno.</param>
        /// <param name="ex">Excepcion de tipo Generico.</param>
        /// <param name="errInterno">Error Interno</param>
        public CoreException(string message, Exception ex, long errInterno)
            : base(message, ex)
        {
            _errInterno = errInterno;
            _message = message;

        }

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        /// <param name="message">Mensaje interno.</param>
        /// <param name="errInterno">Error Interno</param>
        public CoreException(string message, long errInterno)
            : base(message)
        {
            _errInterno = errInterno;
            _message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CoreException(SerializationInfo info,
         StreamingContext context)
            : base(info, context)
        {

        }

        private void CargarErrorInterno(Exception ex)
        {
            if (ex is AppMetrics.Data.ADONETDataAccess.DataAccessException){

                _errInterno = ((AppMetrics.Data.ADONETDataAccess.DataAccessException)ex).errInterno;
            }
            else
            {
                _errInterno = HResult;
            }
        }
    }
}
