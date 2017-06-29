using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AppMetrics.Logger
{
    #region Classes

    public class Log
    {
        #region Constructor
        public Log()
        {
            CreateLog("ACTIVITY.LOG", "ERRRORS.LOG", 4096, 32, true);
        }

        public Log(string activityLog, string errorLog, long logSize, int bufferSize, bool relativePath)
        {
            CreateLog(activityLog, errorLog, logSize, bufferSize, relativePath);
        }

        public Log(string activityLog, long logSize, int bufferSize, bool relativePath)
        {
            CreateLog(activityLog, "", logSize, bufferSize, relativePath);
        }
        #endregion

        #region Variables
        private TextWriterTraceListener activityLogger;
        private TextWriterTraceListener errorLogger;
        private bool logError = true;
        #endregion

        #region Properties
        private string _LogActivity;
        private string _LogError;
        private long _LogSize;
        private int _BufferSize;
        #endregion

        #region Properties Methods
        public string ActivityLog
        {
            get { return _LogActivity; }
        }
        public string ErrorLog
        {
            get { return _LogError; }
        }
        public long LogSize
        {
            get { return _LogSize; }
        }
        public int BufferSize
        {
            get { return _BufferSize; }
        }
        #endregion

        #region Public Members

        #region TraceLog
        public void TraceLog(string message)
        {
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }

        public void TraceLog(string message, int tracking)
        {
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking.ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }

        public void TraceLog(string message, long tracking)
        {
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking.ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }

        public void TraceLog(string message, string tracking)
        {
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }
        #endregion

        #region TraceError

        public void TraceError(string message)
        {
            if (logError) LogError("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }

        public void TraceError(string message, int tracking)
        {
            if (logError) LogError("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking.ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking.ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }

        public void TraceError(string message, long tracking)
        {
            if (logError) LogError("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking.ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking.ToString() + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }

        public void TraceError(string message, string tracking)
        {
            if (logError) LogError("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
            LogActivity("TH: " + Thread.CurrentThread.GetHashCode().ToString() + " - " + tracking + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - MSG: " + message);
        }
        #endregion

        #region CloseLog
        public void CloseLog()
        {
            try
            {
                if (logError) errorLogger.Close();
                activityLogger.Close();
            }
            catch { }
            finally
            {
                errorLogger = null;
                activityLogger = null;
            }
        }
        #endregion

        #endregion

        #region Private Members
        private void CreateLog(string activityLog, string errorLog, long logSize, int bufferSize, bool relativePath)
        {
            string logDirectory;

            try
            {
                if (errorLog == "") logError = false;

                if (relativePath)
                {
                    //Path relativo -> Utilizo el path de la aplicación
                    _LogActivity = AppPath() + "\\" + activityLog;
                    _LogError = AppPath() + "\\" + errorLog;
                }
                else
                {
                    //Path absoluto
                    _LogActivity = activityLog;
                    _LogError = errorLog;
                }

                logDirectory = Path.GetDirectoryName(_LogActivity);
                if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);

                _LogSize = logSize;
                _BufferSize = bufferSize;

                string fecha = String.Format("{0:00}", DateTime.Now.Day) + "-"
                               + String.Format("{0:00}", DateTime.Now.Month) + "-"
                               + String.Format("{0:0000}", DateTime.Now.Year) + " a las "
                               + String.Format("{0:00}", DateTime.Now.Hour) + ":"
                               + String.Format("{0:00}", DateTime.Now.Minute) + ":"
                               + String.Format("{0:00}", DateTime.Now.Second);

                string initLog = "**********************************************************\n" +
                                 "* Session del log iniciada  (" + fecha + ")  *\n" +
                                 "**********************************************************";

                FileStreamWithBackup fileActivity = new FileStreamWithBackup(_LogActivity, _LogSize * 1024, FileMode.Append, FileShare.Read, bufferSize * 1024);
                fileActivity.CanSplitData = false;
                activityLogger = new TextWriterTraceListener(fileActivity);
                activityLogger.WriteLine(initLog);
                activityLogger.Flush();

                if (logError)
                {
                    FileStreamWithBackup fileError = new FileStreamWithBackup(_LogError, _LogSize * 1024, FileMode.Append, FileShare.Read, bufferSize * 1024);
                    fileError.CanSplitData = false;
                    errorLogger = new TextWriterTraceListener(fileError);
                    errorLogger.WriteLine(initLog);
                    errorLogger.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LogActivity(string message)
        {
            Monitor.Enter(activityLogger);
            try
            {
                activityLogger.WriteLine(message);
                activityLogger.Flush();
            }
            catch (Exception ex)
            {
                DumpError("Activity", ex);
            }
            finally
            {
                Monitor.Exit(activityLogger);
            }
        }

        private void LogError(string message)
        {
            Monitor.Enter(errorLogger);
            try
            {
                errorLogger.WriteLine(message);
                errorLogger.Flush();
            }
            catch (Exception ex)
            {
                DumpError("Error", ex);
            }
            finally
            {
                Monitor.Exit(errorLogger);
            }
        }

        private static String AppPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private void DumpError(string fileName, Exception ex)
        {
            StreamWriter streamWriter;
            try
            {
                streamWriter = File.AppendText(AppPath() + "\\Dump.txt");
                streamWriter.WriteLine("Error al escribir en el log '" + fileName + "'\r\n" +
                             "Message: " + ex.Message + "\r\n" +
                             "StackTrace: " + ex.StackTrace + "\r\n" +
                             "Source: " + ex.Source);
                streamWriter.Close();
            }
            catch { }
            finally
            {
                streamWriter = null;
            }
        }
        #endregion
    }
    #endregion
}
