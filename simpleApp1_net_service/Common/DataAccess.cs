using System;
using System.Data;
using System.Threading;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Reflection;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Actinow.Logger;

namespace Actinow.Data.ADONETDataAccess
{
    public class DataAccess : IDisposable
    {
        private string ParamsValues;

        #region constructors / finalizer methods

        public enum PROVIDER_TYPE
        {
            PROVIDER_NONE,
            PROVIDER_SQLCLIENT
        }

        /// <summary>
        /// Inicialización de variables en el contructor de la clase
        /// </summary>
        private DataAccess()
        {
            Inicializa();
        }

        /// <summary>
        /// Inicialización de variables en el contructor de la clase
        /// </summary>
        private DataAccess(ICacheManager cManager, Log oLog)
        {
            m_cManager = cManager;
            m_oLog = oLog;
            Inicializa();
        }

        ~DataAccess()
        {
            try
            {
                this.Dispose(false);
            }
            catch
            {

            }
        }

        private void Inicializa()
        {
            m_oConnection = null;
            m_oCommand = null;
            m_oTransaction = null;
            m_sConnectionString = null;
            m_nroTransaction = 0;
            m_nCommandTimeout = 0;
            m_nRetryConnect = 3;
            m_bDisposed = false;
            m_bConnected = false;
            m_sProviderAssembly = null;
            m_sProviderConnectionClass = null;
            m_sProviderCommandBuilderClass = null;
            m_eProvider = PROVIDER_TYPE.PROVIDER_NONE;
            m_idConexion = 0;
            m_oDataReader = null;
        }

        #endregion

        #region private/protected members

        private System.Data.IDataReader m_oDataReader;
        private System.Data.IDbConnection m_oConnection;
        private System.Data.IDbCommand m_oCommand;
        private System.Data.IDbTransaction m_oTransaction;
        private int m_nroTransaction;
        private string m_sConnectionString;
        private int m_nCommandTimeout;
        private int m_nRetryConnect;
        private bool m_bDisposed;
        private bool m_bConnected;
        private string m_sProviderAssembly;
        private string m_sProviderConnectionClass;
        private string m_sProviderCommandBuilderClass;
        private PROVIDER_TYPE m_eProvider;
        private ICacheManager m_cManager;
        private Log m_oLog;
        private int m_idConexion;

        #endregion

        #region Database connect / DisConnect / Transaction methods

        /// <summary>
        /// Valida si existe o no una conexión
        /// </summary>
        /// <returns>bool</returns>
        public bool ValidateConnection()
        {
            try
            {
                if (m_bConnected)
                {
                    return true;
                }
                else
                {
                    return Connect();
                }
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al validar la conexión.", ex);
            }
        }

        public void ValidateDataReader()
        {
            try
            {
                if (m_oDataReader != null && !m_oDataReader.IsClosed)
                {
                    m_oDataReader.Close();
                    m_oDataReader.Dispose();
                    m_oDataReader = null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("No se ha podido cerrar al DataReader.", ex);
            }
        }

        /// <summary>
        /// Realiza una conexión con la Base de Datos
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                // Validar si el 'ConnectionString' es válido
                if (m_sConnectionString == null || m_sConnectionString.Length == 0)
                {
                    if (m_oLog != null)
                    {
                        m_oLog.TraceError("La cadena de conexión para la Base de Datos no es válida.", m_idConexion);
                    }
                    throw (new DataAccessException("La cadena de conexión para la Base de Datos no es válida.", -50));
                }

                // Desconectarse si esta actualmente conectado
                Disconnect();

                // Obtener el objeto ADO.NET Conection
                m_oConnection = GetConnection();
                m_oConnection.ConnectionString = m_sConnectionString;

                // Intentar conectar
                for (int i = 0; i <= m_nRetryConnect; i++)
                {
                    if (m_oLog != null)
                    {
                        if (i > 0) m_oLog.TraceLog("Intento de conexion nro: " + i.ToString(), m_idConexion);
                    }

                    try
                    {
                        m_oConnection.Open();

                        if (m_oConnection.State == ConnectionState.Open)
                        {
                            m_bConnected = true;
                            break;
                        }
                    }
                    catch
                    {
                        if (i == m_nRetryConnect)
                            throw;

                        // Reintentos cada 1 segundo
                        Thread.Sleep(1000);
                    }
                }

                // Obtiene el objeto COMMAND
                m_oCommand = m_oConnection.CreateCommand();
                m_oCommand.CommandTimeout = (m_nCommandTimeout > 0) ? m_nCommandTimeout : m_oConnection.ConnectionTimeout;

                return m_bConnected;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error no esperado al realizar la conexión.", ex);
            }
        }

        /// <summary>
        /// Realiza la desconexión de la Base de Datos
        /// </summary>
        public void Disconnect()
        {
            try
            {
                ValidateDataReader();

                bool OpenTransaction = false;
                // Disconnect puede llamarse desde 'Dispose' y debe garantizar que no hay errores
                if (!m_bConnected)
                {
                    return;
                }

                // Si quedaron transacciones abiertas, realiza el Rollback
                if (m_oTransaction != null)
                {
                    if (m_oLog != null)
                    {
                        m_oLog.TraceLog("Al desconectar se detectaron transacciones abiertas...", m_idConexion);
                    }

                    RollbackTransaction(true);
                    OpenTransaction = true;
                }

                // Elimina el objeto Command
                if (m_oCommand != null)
                {
                    m_oCommand.Dispose();
                    m_oCommand = null;
                }

                // Elimina el objeto Connection
                if (m_oConnection != null)
                {
                    // Intenta cerrar la conexión
                    try
                    {
                        m_oConnection.Close();
                    }
                    catch
                    {

                    }

                    m_oConnection.Dispose();
                    m_oConnection = null;
                }

                m_bConnected = false;

                if (OpenTransaction)
                {
                    throw new DataAccessException("Se han detectado una o mas Transacciones abiertas al momento de desconectarse de la Base de Datos. No se ha podido completar la operación.", -30);
                }
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al desconectase de la Base de Datos.", ex);
            }
        }

        /// <summary>
        /// Intenta la desconexión de la Base de Datos
        /// </summary>
        public bool TryDisconnect()
        {
            try
            {
                Disconnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Comienza una nueva Transacción. 
        /// El primer BEGIN TRANSACTION es real, los demás son ficticios
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                if (m_oLog != null)
                {
                    //m_oLog.TraceLog("Iniciando transacción...");
                }

                //Si no existe ninguna transaccion, realiza el BEGIN TRANSACTION real
                if (m_nroTransaction == 0)
                {
                    ValidateConnection();

                    m_oTransaction = m_oConnection.BeginTransaction();
                    m_oCommand.Transaction = m_oTransaction;
                    //m_oCommand.CommandTimeout = 120;
                }

                // Incremento nro de transacciones abiertas
                m_nroTransaction++;

                return;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al iniciar la transacción.", ex);
            }
        }

        /// <summary>
        /// Realiza el COMMIT de las transacciones. Solo cuando 
        /// queda UNA SOLA transacción abierta, se realiza el COMMIT real.
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                if (m_oTransaction == null)
                {
                    if (m_oLog != null)
                    {
                        m_oLog.TraceError("Error al realizar Commit porque no se encontraron transacciones abiertas...", m_idConexion);
                    }

                    throw (new DataAccessException("BeginTransaction se debe llamar antes de un COMMIT o ROLLBACK. No se encontraron transacciones abiertas.", -35));
                }

                //Si queda solo una transacción abierta
                if (m_nroTransaction == 1)
                {
                    m_oTransaction.Commit();
                    m_oTransaction.Dispose();
                    m_oTransaction = null;
                }

                // Decremento nro de transacciones pendientes
                m_nroTransaction--;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al finalizar la transacción.", ex);
            }
        }

        /// <summary>
        /// Realiza el Rollback de las Transacciones abiertas
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Realizando Rollback...", m_idConexion);
                }

                if (m_nroTransaction != 0)
                {
                    RollbackTransaction(true);
                }
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al cancelar la transacción.", ex);
            }
        }

        /// <summary>
        /// Realiza el Rollback de las Transacciones Abiertas.
        /// </summary>
        /// <param name="bThrowError">Permitir o no el THROW de los errores</param>
        public void RollbackTransaction(bool bThrowError)
        {
            if (m_oTransaction == null)
            {
                if (bThrowError)
                    throw (new DataAccessException("BeginTransaction se debe llamar antes de un commit o rollback. No se encontraron transacciones abiertas.", -35));
            }

            try
            {
                m_oTransaction.Rollback();
                m_oTransaction.Dispose();
                m_oTransaction = null;
            }
            catch (DataAccessException ex)
            {
                if (bThrowError)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                if (bThrowError)
                {
                    throw new DataAccessException("Error inesperado al finalizar la transacción.", ex);
                }
            }
            finally
            {
                m_nroTransaction = 0;
            }
        }

        #endregion

        #region Wraper methods for ADO.NET

        /// <summary>
        /// Obtiene un DataTable alojado en el cache de Aplicacion
        /// </summary>
        /// <param name="strSQL">String del SQL q se encuentra asociado en el Cache</param>
        /// <param name="TimeOut">Tiempo de Expiracion en Minutos</param>
        /// <param name="PropertyName">Nombre de la Propiedad a alojar en el Cache. Si se recibe una cadena vacía, se utiliza una hash del parámetro strSQL como PropertyName</param>
        /// <param name="AbsoluteExpiration">Si es TRUE -> 'Expiracion Absoluta', si el FALSE, 'EXPIRACION RELATIVA'</param>
        /// <param name="minutes">Si el tiempo expresado es en Minutos.</param>
        /// <returns>DataTable alojado en Cache.</returns>
        public DataTable ExecuteQueryCache(string strSQL, long TimeOut, string PropertyName, bool AbsoluteExpiration, bool minutes)
        {
            try
            {
                return ExecuteQueryCache(strSQL, TimeOut, PropertyName, AbsoluteExpiration, minutes, true);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al ejecutar la Query Cache.", ex);
            }
        }

        /// <summary>
        /// Obtiene un DataTable alojado en el cache de Aplicacion
        /// </summary>
        /// <param name="strSQL">String del SQL q se encuentra asociado en el Cache</param>
        /// <param name="TimeOut">Tiempo de Expiracion en Minutos</param>
        /// <param name="PropertyName">Nombre de la Propiedad a alojar en el Cache. Si se recibe una cadena vacía, se utiliza una hash del parámetro strSQL como PropertyName</param>
        /// <param name="AbsoluteExpiration">Si es TRUE -> 'Expiracion Absoluta', si el FALSE, 'EXPIRACION RELATIVA'</param>
        /// <param name="minutes">Si el tiempo expresado es en Minutos.</param>
        /// <param name="getCopy">Indica si devuelve una copia del objeto o el original.</param>
        /// <returns>DataTable alojado en Cache.</returns>
        public DataTable ExecuteQueryCache(string strSQL, long TimeOut, string PropertyName, bool AbsoluteExpiration, bool minutes, bool getCopy)
        {
            object oResult = null;

            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Obteniendo datos del Cache...", m_idConexion);
                }

                // Obtener PropertyName del QueryString
                if (PropertyName == "")
                {
                    PropertyName = getQueryHash(strSQL);
                }

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("PropertyName->" + PropertyName, m_idConexion);
                }

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("TimeOut->" + TimeOut.ToString(), m_idConexion);
                }

                oResult = m_cManager.GetData(PropertyName);
                if (oResult != null && ((DataTable)oResult).Columns.Count == 0)
                {
                    oResult = null;
                }

                if (oResult == null)
                {
                    lock (m_cManager)
                    {
                        oResult = m_cManager.GetData(PropertyName);
                        if (oResult != null && ((DataTable)oResult).Columns.Count == 0)
                        {
                            oResult = null;
                        }

                        if (oResult != null)
                        {
                            if (getCopy)
                            {
                                return ((DataTable)oResult).Copy();
                            }
                            else
                            {
                                return (DataTable)oResult;
                            }
                        }

                        oResult = GetDataTable(strSQL);

                        if (AbsoluteExpiration)
                        {
                            m_cManager.Add(PropertyName, oResult,
                                            CacheItemPriority.Normal,
                                            null,
                                            new AbsoluteTime(TimeSpan.FromSeconds(TimeOut * (minutes ? 60 : 1))));
                        }
                        else
                        {
                            m_cManager.Add(PropertyName, oResult,
                                            CacheItemPriority.Normal,
                                            null,
                                            new SlidingTime(TimeSpan.FromSeconds(TimeOut * (minutes ? 60 : 1))));
                        }
                    }
                }

                if (getCopy)
                {
                    return ((DataTable)oResult).Copy();
                }
                else
                {
                    return (DataTable)oResult;
                }
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceError("ExecuteQueryCache -> ArgumentOutOfRangeException: " + ex.Message, m_idConexion);
                }
                if (getCopy)
                {
                    return ((DataTable)oResult).Copy();
                }
                else
                {
                    return (DataTable)oResult;
                }

            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al ejecutar la Query Cache.", ex);
            }
        }

        private string getQueryHash(string strSQL)
        {
            try
            {
                byte[] sourceBytes;
                SHA256 propertyHash;
                byte[] hashBytes;

                sourceBytes = System.Text.Encoding.Default.GetBytes(strSQL);
                propertyHash = new SHA256Managed();
                hashBytes = propertyHash.ComputeHash(sourceBytes);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; hashBytes != null && i < hashBytes.Length; i++)
                {
                    sb.AppendFormat("{0:x2}", hashBytes[i]);
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el HASH de la query ingresada.", ex);
            }
        }

        /// <summary>
        /// Ejecuta una Query String, El commandType es siempre Text
        /// </summary>
        /// <param name="sSQL">QueryString pasada como parametro</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteReader(string sSQL)
        {
            try
            {
                return ExecuteReader(ProcessQryString(sSQL, CommandType.Text), CommandType.Text);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al realizar la consulta.", ex);
            }
        }

        /// <summary>
        /// Ejecuta una QueryString o un Store Procedure
        /// </summary>
        /// <param name="sSQL">Nombre del StoreProcedure o QueryString</param>
        /// <param name="oType">Tipo de Comando a Ejecuatr</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteReader(string sSQL, CommandType oType)
        {
            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Realizando ExecuteReader...", m_idConexion);
                }

                ValidateDataReader();

                ValidateConnection();

                m_oCommand.CommandText = ProcessQryString(sSQL, oType);
                m_oCommand.CommandType = oType;

                ProcessParametersString();

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("QueryString -> " + m_oCommand.CommandText, m_idConexion);
                    m_oLog.TraceLog("ParamsValues -> " + ParamsValues, m_idConexion);
                }

                m_oDataReader = m_oCommand.ExecuteReader();

                return m_oDataReader;
            }
            catch (SqlException ex)
            {
                throw (new DataAccessException("Hubo un error en la Consulta. <br/> Consulta: " + sSQL + " <br/> Parametros: " + ParamsValues, ex));
            }
            catch (DataAccessException ex)
            {
                throw (ex);
            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Hubo un error inesperado al tratar de realizar la consulta.", ex));
            }
        }

        /// <summary>
        /// Ejecuta una QueryString, el CommandType es siempre Text
        /// </summary>
        /// <param name="sSQL">QueryString</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sSQL)
        {
            try
            {
                DataTable oData = new DataTable();
                return GetDataTable(sSQL, CommandType.Text, oData);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el DataTable.", ex);
            }
        }

        /// <summary>
        /// Ejecuta una QueryString o un Store Procedure
        /// </summary>
        /// <param name="sSQL">Nombre del StoreProcedure o QueryString</param>
        /// <param name="oType">Tipo de Comando a Ejecutar</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sSQL, CommandType oType)
        {
            try
            {
                DataTable oData = new DataTable();
                return GetDataTable(sSQL, oType, oData);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el DataTable.", ex);
            }
        }

        /// <summary>
        /// Ejecuta una QueryString o un Store Procedure
        /// </summary>
        /// <param name="sSQL">Nombre del StoreProcedure o QueryString</param>
        /// <param name="oType">Tipo de Comando a Ejecutar</param>
        /// <param name="oData">Data Set donde se agregan los datos</param>
        /// <returns>DataSet</returns>
        public DataTable GetDataTable(string sSQL, CommandType oType, DataTable oData)
        {
            SqlDataAdapter oAdpt = null;

            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Obteniendo DataTable...", m_idConexion);
                }

                ValidateDataReader();

                ValidateConnection();

                m_oCommand.CommandType = oType;
                m_oCommand.CommandText = ProcessQryString(sSQL, oType);

                ProcessParametersString();

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("QueryString -> " + m_oCommand.CommandText, m_idConexion);
                    m_oLog.TraceLog("ParamsValues -> " + ParamsValues, m_idConexion);
                }

                oAdpt = new SqlDataAdapter(sSQL, (SqlConnection)m_oConnection);
                oAdpt.SelectCommand = (SqlCommand)m_oCommand;
                oAdpt.Fill(oData);

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("El DataTable se ha obtenido satisfactoriamente...", m_idConexion);
                }

                return oData;
            }
            catch (SqlException ex)
            {
                throw (new DataAccessException("Hubo un error en la Consulta. <br/> Consulta: " + sSQL + " <br/> Parametros: " + ParamsValues, ex));

            }
            catch (DataAccessException ex)
            {
                throw (ex);
            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Hubo un error inesperado al tratar de realizar la consulta.", ex));
            }
            finally
            {
                if (oAdpt != null)
                {
                    oAdpt.Dispose();
                    oAdpt = null;

                }
            }
        }

        /// <summary>
        /// Ejecura una Query y devuelve la primer columna de la primer fila, las demas filas y columnas son ignoradas
        /// </summary>
        /// <param name="sSQL">QueryString a ejecutar</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string sSQL)
        {
            try
            {
                return ExecuteScalar(sSQL, CommandType.Text);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al ejecutar la consulta.", ex);
            }
        }

        /// <summary>
        /// Ejecura una Query o Store Procedure, y devuelve la primer columna de la primer fila, las demas filas y columnas son ignoradas
        /// </summary>
        /// <param name="sSQL">Nombre de Store Procedure o Query a ejecutar</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string sSQL, CommandType oType)
        {
            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Realizando ExecuteScalar...", m_idConexion);
                }

                ValidateDataReader();

                ValidateConnection();

                m_oCommand.CommandText = ProcessQryString(sSQL, oType);
                m_oCommand.CommandType = oType;

                ProcessParametersString();

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("QueryString -> " + m_oCommand.CommandText, m_idConexion);
                    m_oLog.TraceLog("ParamsValues -> " + ParamsValues, m_idConexion);
                }

                return m_oCommand.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw (new DataAccessException("Hubo un error en la Consulta. <br/> Consulta: " + sSQL + " <br/> Parametros: " + ParamsValues, ex));

            }
            catch (DataAccessException ex)
            {
                throw (ex);
            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Hubo un error inesperado al tratar de realizar la consulta.", ex));
            }
        }

        /// <summary>
        /// Ejecuta un INSERT, UPDATE o DELETE (formato string) y devuelve las filas afectadas
        /// </summary>
        /// <param name="sSQL">Query a ejecutar</param>
        /// <returns>Devuelve Nro de filas afectadas</returns>
        public object ExecuteNonQuery(string sSQL)
        {
            try
            {
                return ExecuteNonQuery(sSQL, CommandType.Text);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al ejecutar la consulta.", ex);
            }
        }

        /// <summary>
        /// Ejecuta un INSERT, UPDATE o DELETE (formato Query o Store Procedure) y devuelve las filas afectadas
        /// </summary>
        /// <param name="sSQL">Query o Store Procedure a ejecutar</param>
        /// <returns>Devuelve Nro de filas afectadas</returns>
        public object ExecuteNonQuery(string sSQL, CommandType oType)
        {
            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Realizando ExecuteNonQuery...", m_idConexion);
                }

                ValidateDataReader();

                ValidateConnection();

                m_oCommand.CommandText = ProcessQryString(sSQL, oType);
                m_oCommand.CommandType = oType;

                ProcessParametersString();

                if (m_oLog != null)
                {
                    m_oLog.TraceLog("QueryString -> " + m_oCommand.CommandText, m_idConexion);
                    m_oLog.TraceLog("ParamsValues -> " + ParamsValues, m_idConexion);
                }

                return m_oCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw (new DataAccessException("Hubo un error en la Consulta. <br/> Consulta: " + sSQL + " <br/> Parametros: " + ParamsValues, ex));

            }
            catch (DataAccessException ex)
            {
                throw (ex);
            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Hubo un error inesperado al tratar de realizar la consulta.", ex));
            }
        }

        /// <summary>
        /// Ejecuta un INSERT(formato string) y devuelve el ID del registro insertado
        /// </summary>
        /// <param name="sSQL">Query a ejecutar</param>
        /// <returns>ID del Registro insertado</returns>
        public object ExecuteIdentity(string sSQL)
        {
            int resultado;

            try
            {
                if (m_oLog != null)
                {
                    m_oLog.TraceLog("Realizando ExecuteIdentity...", m_idConexion);
                }

                resultado = (int)ExecuteNonQuery(sSQL, CommandType.Text);

                if (resultado == 1)
                {
                    m_oCommand.CommandText = "SELECT @@IDENTITY ";
                    m_oCommand.CommandType = CommandType.Text;
                    return m_oCommand.ExecuteScalar();
                }
                else
                {
                    return -1;
                }
            }
            catch (SqlException ex)
            {
                throw (new DataAccessException("Hubo un error al ejecutar el comando. <br/> Consulta: " + sSQL + " <br/> Parametros: " + ParamsValues, ex));

            }
            catch (DataAccessException ex)
            {
                throw (ex);
            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Hubo un error inesperado al tratar de ejecutar el comando.", ex));
            }
        }

        /// <summary>
        /// Crea un parametro
        /// </summary>
        /// <param name="parameterName">Nombre del Parametro</param>
        /// <param name="parameterValue">Valor del Parametro</param>
        /// <returns>IDataParameter</returns>
        private IDataParameter CreateParameter(string parameterName, object parameterValue)
        {
            try
            {
                return new SqlParameter("@" + parameterName, parameterValue == null ? DBNull.Value : parameterValue);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al crear el Parametro.", ex);
            }
        }

        /// <summary>
        /// Devuelve los parametros del Command a ejecutar
        /// </summary>
        /// <returns>IDataParameterCollection</returns>
        private IDataParameterCollection GetParameters()
        {
            try
            {
                ValidateConnection();

                return m_oCommand.Parameters;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener la colección de parametros.", ex);
            }
        }

        /// <summary>
        /// Agrega un parametro al Command a ejecutar
        /// </summary>
        /// <param name="parameterName">Nombre del Parametro</param>
        /// <param name="parameterValue">Valor del Parametro</param>
        public void AddParameter(string parameterName, Object parameterValue)
        {
            try
            {
                ValidateConnection();

                m_oCommand.Parameters.Add(CreateParameter(parameterName, parameterValue));
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al agregar el parametro " + parameterName + " = '" + parameterValue.ToString() + "'", ex);
            }
        }

        /// <summary>
        /// Elimina todos los parametros del Command a Ejecutar
        /// </summary>
        public void ClearParameters()
        {
            try
            {
                if (m_oCommand != null)
                    m_oCommand.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al borrar los parametros", ex);
            }
        }

        public static DataAccess GetSqlClientWrapper(string dataSource, string initialCatalog, string userId, string password, int connectTimeout, string applicationName, ICacheManager oCache, Log oLog)
        {
            try
            {
                DataAccess oDB = new DataAccess(oCache, oLog);
                oDB.m_sConnectionString = GetConnectionString(dataSource, initialCatalog, userId, password, connectTimeout, applicationName, "0", "0", "100");
                oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_SQLCLIENT;
                oDB.CommandTimeout = Convert.ToInt32(connectTimeout);

                return oDB;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el Wrapper.", ex);
            }
        }

        public static DataAccess GetSqlClientWrapper(string dataSource, string initialCatalog, string userId, string password, int connectTimeout, string applicationName, string connectionLifeTime, string minPoolSize, string maxPoolSize, ICacheManager oCache, Log oLog)
        {
            try
            {
                DataAccess oDB = new DataAccess(oCache, oLog);
                oDB.m_sConnectionString = GetConnectionString(dataSource, initialCatalog, userId, password, connectTimeout, applicationName, connectionLifeTime, minPoolSize, maxPoolSize);
                oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_SQLCLIENT;
                oDB.CommandTimeout = Convert.ToInt32(connectTimeout);

                return oDB;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el Wrapper.", ex);
            }
        }

        public static DataAccess GetSqlClientWrapper(string connectionString, ICacheManager oCache, Log oLog)
        {
            try
            {
                DataAccess oDB = new DataAccess(oCache, oLog);
                oDB.m_sConnectionString = connectionString;
                oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_SQLCLIENT;

                return oDB;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el Wrapper.", ex);
            }
        }

        private static string GetConnectionString(string dataSource, string initialCatalog, string userId, string password, int connectTimeout, string applicationName, string connectionLifeTime, string minPoolSize, string maxPoolSize)
        {
            try
            {
                return "data source=" + dataSource + ";initial catalog=" + initialCatalog + ";user id=" + userId + ";password=" + password + ";connect timeout=" + connectTimeout.ToString() + ";application name=" + applicationName + ";persist security info=False;Min Pool Size=" + minPoolSize + ";Max Pool Size=" + maxPoolSize + ";Connection Lifetime=" + connectionLifeTime;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el string de conexión.", ex);
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al realizar el Dispose.", ex);
            }
        }

        /// <summary>
        /// Lo que sigue no en un método de interfaz IDisposable.  
        /// Pero se añadió en esta región ya que se relaciona más con Dispose.
        /// </summary>
        /// <param name="bDisposing"></param>
        protected void Dispose(bool bDisposing)
        {
            try
            {
                if (!m_bDisposed)
                {
                    // Dispose en bloques, sólo recursos administrados
                    if (bDisposing)
                    {
                        this.Disconnect();
                    }
                }

                m_bDisposed = true;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al realizar el Dispose.", ex);
            }
        }

        #endregion

        #region properties (get/set methods)

        public int IdConexion
        {
            get
            {
                return m_idConexion;
            }
            set
            {
                m_idConexion = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return m_sConnectionString;
            }
            set
            {
                m_sConnectionString = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return m_nCommandTimeout;
            }
            set
            {
                m_nCommandTimeout = value;
            }
        }

        public int ConnectionRetryCount
        {
            get
            {
                return m_nRetryConnect;
            }
            set
            {
                m_nRetryConnect = value;

                if (m_nRetryConnect <= 0)
                    m_nRetryConnect = 0;
            }
        }

        public string ProviderAssemblyName
        {
            get
            {
                return m_sProviderAssembly;
            }
            set
            {
                m_sProviderAssembly = value;
            }
        }

        public string ProviderConnectionClassName
        {
            get
            {
                return m_sProviderConnectionClass;
            }
            set
            {
                m_sProviderConnectionClass = value;
            }
        }

        public string ProviderCommandBuilderClassName
        {
            get { return m_sProviderCommandBuilderClass; }
            set { m_sProviderCommandBuilderClass = value; }
        }

        public PROVIDER_TYPE PROVIDER
        {
            get { return m_eProvider; }
        }

        #endregion

        #region Utility functions

        /// <summary>
        /// Genera el objeto Connection
        /// </summary>
        /// <returns>IDbConnection</returns>
        protected IDbConnection GetConnection()
        {
            try
            {
                IDbConnection oReturn = null;

                oReturn = new SqlConnection();

                if (oReturn == null)
                    throw (new DataAccessException("Failed to get ADONET Connection object [IDbConnection]", -115));

                return oReturn;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener la Conexion.", ex);
            }
        }

        /// <summary>
        /// Devuelve los parametros del Store Procedure especificado
        /// </summary>
        /// <param name="sStoredProcedure">Nombre del Store Prodecure</param>
        /// <returns>IDataParameterColleccion</returns>
        public IDataParameterCollection DeriveParameters(string sStoredProcedure)
        {
            return DeriveParameters(sStoredProcedure, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Devuelve los Parametros del StoreProcedure o QueryString especificada.
        /// </summary>
        /// <param name="sStoredProcedure">Nombre del Store Prodecure o query</param>
        /// <returns>IDataParameterColleccion</returns>
        public IDataParameterCollection DeriveParameters(string sSql, CommandType oType)
        {
            ValidateConnection();

            ClearParameters();

            m_oCommand.CommandText = ProcessQryString(sSql, oType);
            m_oCommand.CommandType = oType;

            SqlCommandBuilder.DeriveParameters((SqlCommand)m_oCommand);

            return m_oCommand.Parameters;
        }

        protected object GetADONETProviderObject(string sAssembly, string sClass, object[] oArgs)
        {
            if (sAssembly == null || sAssembly.Trim().Length == 0)
                throw (new DataAccessException("Invalid provider assembly name", -22));

            if (sClass == null || sClass.Trim().Length == 0)
                throw (new DataAccessException("Invalid provider connection class name", -22));

            Assembly oSrc = Assembly.Load(sAssembly);
            if (oArgs == null)
            {
                return oSrc.CreateInstance(sClass, true);
            }
            else
            {
                Type oType = oSrc.GetType(sClass, true, true);
                Type[] arTypes = new Type[oArgs.Length];
                for (int i = 0; i < oArgs.Length; i++)
                {
                    arTypes[i] = oArgs[0].GetType();
                }
                ConstructorInfo oConstr = oType.GetConstructor(arTypes);

                return oConstr.Invoke(oArgs);
            }
        }

        /// <summary>
        /// Reemplaza los '?' de la QueryString por el nombre de los parametros correspondientes.
        /// </summary>
        /// <param name="strQry">QueryString</param>
        /// <param name="oType">Tipo de Command, si es Store procedure no lo modifica</param>
        /// <returns>QueryString Generada</returns>
        public String ProcessQryString(String strQry, CommandType oType)
        {
            try
            {
                string dest = "";
                dest = strQry;

                if (oType == CommandType.Text)
                {
                    int pos = 0, lastPos = 0, i = 0;
                    string source = strQry;

                    pos = source.IndexOf("?", pos);
                    if (pos > 0) dest = "";
                    while (pos > 0)
                    {
                        dest += source.Substring(lastPos, pos - lastPos) + m_oCommand.Parameters[i];
                        lastPos = pos + 1;
                        pos = source.IndexOf("?", lastPos);
                        if (pos > 0)
                        {
                            i++;
                        }
                        else
                        {
                            if (lastPos < source.Length)
                            {
                                string aver = source.Substring(lastPos, source.Length - lastPos);
                                dest += source.Substring(lastPos, source.Length - lastPos);
                            }
                        }
                    }
                }
                return dest;

            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Valide la cantidad de parametros ingresados en string.", ex, -100));
            }

        }

        public void ProcessParametersString()
        {
            try
            {
                ParamsValues = "";
                foreach (SqlParameter param in m_oCommand.Parameters)
                {
                    System.Diagnostics.Trace.WriteLine(param.ParameterName + " = '" + param.SqlValue + "' | " + param.Value);

                    ParamsValues += param.ParameterName + " = '" + param.Value + "'";
                }
            }
            catch (Exception ex)
            {
                throw (new DataAccessException("Valide la cantidad de parametros ingresados en string.", ex));
            }
        }

        /// <summary>
        /// Analiza si busca en la tabla o en la vista
        /// </summary>
        /// <param name="tableName">Nombre de la Tabla</param>
        /// <param name="fecHasta">Fecha Hasta</param>
        /// <returns></returns>
        public string GetTableName(string tableName, DateTime? fecHasta)
        {
            try
            {
                int MonthsInProd = Convert.ToInt32(GetValorParametro("MonthsInProd"));

                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);

                if (fecHasta == null)
                {
                    return tableName;
                }
                else
                {
                    if (fecHasta >= date.AddMonths(-MonthsInProd))
                    {
                        return tableName;
                    }
                    else
                    {
                        return "vw" + tableName;
                    }
                }
            }
            catch
            {
                return tableName;
            }
        }

        /// <summary>
        /// Obtiene el Valor de un Parametro
        /// </summary>
        /// <param name="prmClave">Clave del Parametro</param>
        /// <returns>Valor del Parametro</returns>
        private string GetValorParametro(string prmClave)
        {
            string strSQL = "";
            SqlDataReader dataRdr = null;
            string prmValue = "";

            try
            {
                ClearParameters();
                AddParameter("idTipoParametro", 1);
                AddParameter("prmClave", prmClave);

                strSQL = " SELECT prmValor " +
                         " FROM Parametros WITH(NOLOCK) " +
                         " WHERE idTipoParametro = ? " +
                         " AND prmClave = ? ";

                dataRdr = (SqlDataReader)ExecuteReader(strSQL);

                if (!dataRdr.HasRows)
                {
                    dataRdr.Close();
                    throw new DataAccessException("No se ha podido obtener el valor del Parametro seleccionado (" + prmClave + ")", 1);
                }

                dataRdr.Read();
                prmValue = dataRdr["prmValor"].ToString();
                dataRdr.Close();

                return prmValue;
            }
            catch (DataAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error inesperado al obtener el valor del Parametro seleccionado (" + prmClave + ")", ex);
            }
            finally
            {
                // Finalizo Varianbles
                if (dataRdr != null && !dataRdr.IsClosed)
                {
                    dataRdr.Close();
                }
            }
        }

        /// <summary>
        /// Obtiene el valor de un campo cadena de un DataReder
        /// </summary>
        /// <param name="readerValue">Campo del DataReder</param>
        /// <returns>Valor del campo del DataReader o una cadena vacía si este es null</returns>
        public static Object CCad(Object readerValue)
        {
            try
            {
                if (readerValue != System.DBNull.Value)
                {
                    return readerValue;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Obtiene el valor de un campo numerico de un DataReder
        /// </summary>
        /// <param name="readerValue">Campo del DataReder</param>
        /// <returns>Valor del campo del DataReader o cero si este es null</returns>
        public static Object CNum(Object readerValue)
        {
            try
            {
                if (readerValue != System.DBNull.Value)
                {
                    return readerValue;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Coloca en 'null' el objeto alojado en el cache
        /// </summary>
        /// <param name="PropertyName">Propiedad a limpiar.</param>
        public void ClearCache(string PropertyName)
        {
            try
            {
                m_cManager.Add(PropertyName, null,
                               CacheItemPriority.Normal,
                               null,
                               new SlidingTime(TimeSpan.FromSeconds(1)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}