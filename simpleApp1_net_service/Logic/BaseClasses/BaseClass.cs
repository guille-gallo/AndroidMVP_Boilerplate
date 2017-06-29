using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using AppMetrics.Common;
using AppMetrics.Data;
using AppMetrics.Logger;
using AppMetrics.Data.ADONETDataAccess;

namespace AppMetrics.Logic.BaseClass
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BaseClass : IDisposable
    {
        private bool isDisposed = false;
        private BizServer _oBizServer;
        private DataAccess _oDataAccess;
        private bool _keepConnectionAlive = false;

        /// <summary>
        /// Devuelve el objeto BizServer.
        /// </summary>
        protected BizServer oBizServer
        {
            set{_oBizServer = value;}
            get{return _oBizServer;}
        }

        /// <summary>
        /// Devuelve el objeto BizServer.DataAccess.
        /// </summary>
        protected DataAccess oDataAccess
        {
            set { _oDataAccess = value; }
            get { return _oDataAccess; }
        }

        /// <summary>
        /// Devuelve el objeto BizServer.Usuario.
        /// </summary>
        protected Usuario oUsuario
        {
            get{return _oBizServer.Usuario;}
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    #region Code to dispose the managed resources of the class

                    #endregion
                }
            }

            #region Code to dispose the un-managed resources of the class

            if (_oDataAccess != null && !_keepConnectionAlive)
            {
                _oDataAccess.TryDisconnect();
                _oDataAccess = null;
            }

            #endregion

            isDisposed = true;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Constructor (Solo para uso Externo) de la Clase BaseClass.
        /// </summary>
        /// <param name="bizSrv"></param>
        /// <param name="callingAssembly"></param>
        /// <param name="creteDBConnection"></param>
        protected BaseClass(BizServer bizSrv, string callingAssembly, bool createDBConnection)
        {
            if (callingAssembly == Assembly.GetExecutingAssembly().GetName().Name)
                throw new Exception("This constructor must be used only from external assembly.");

            try
            {
                if (bizSrv == null)
                    throw new ArgumentNullException("BizServer");

                _oBizServer = bizSrv;

                if (createDBConnection)
                {
                    _oDataAccess = DataAccess.GetSqlClientWrapper(bizSrv.DataBase.ConnectionString, new CacheHelper(), bizSrv.Log);
                    
                    if (bizSrv.Usuario != null)
                        _oDataAccess.TrackingInfo = bizSrv.Usuario.RemoteEndpoint;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Constructor de la Clase BaseClass.
        /// <param name="bizSrv">objeto BizServer</param>
        /// <param name="datAcc">objeto DataAccess</param>
        /// </summary>
        protected BaseClass(BizServer bizSrv, DataAccess datAcc)
        {
            try
            {
                if (bizSrv == null)
                    throw new ArgumentNullException("BizServer");

                if (datAcc == null)
                    throw new ArgumentNullException("DataAccess");

                _oBizServer = bizSrv;
                _oDataAccess = datAcc;
                _keepConnectionAlive = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// CustomException
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Exception CustomException(Exception ex, string message)
        {
            if (ex is DataAccessException)
            {
                return new CoreException(message + "(" + ex.Message + ": " + ex.StackTrace + ")", ex);
            }
            else if (ex is CoreException)
            {
                return ex;
            }
            else if (ex is ValidationException)
            {
                return ex;
            }
            else
            {
                return new CoreException(message + "(" + ex.Message + ": " + ex.StackTrace + ")", ex);
            }
        }
    }
}