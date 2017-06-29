using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WCFAppMetrics
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AppMetrics.Logger.Log logger = null;

            try
            {

                string activity;
                string errors;
                int logSize = AppMetrics.Common.Utils.Utilities.ReadAppSettings<int>(System.Configuration.ConfigurationManager.AppSettings, "LogSize", 4096);
                int logBufferSize = AppMetrics.Common.Utils.Utilities.ReadAppSettings<int>(System.Configuration.ConfigurationManager.AppSettings, "LogBufferSize", 32);
                string PathForLogs = AppMetrics.Common.Utils.Utilities.ReadAppSettings<string>(System.Configuration.ConfigurationManager.AppSettings, "LogPath", string.Empty);

                if (string.IsNullOrEmpty(PathForLogs))
                {
                    activity = Server.MapPath(@"~\App_Data") + "\\ACTIVITY.LOG";
                    errors = Server.MapPath(@"~\App_Data") + "\\ERRORS.LOG";
                }
                else
                {
                    activity = PathForLogs + "\\ACTIVITY.LOG";
                    errors = PathForLogs + "\\ERRORS.LOG";
                }   

                //Build Logger
                logger = new AppMetrics.Logger.Log(activity, errors, 4096, 32, false);

                //Build BizServer
                var bizServer = new AppMetrics.Common.BizServer();
                bizServer.Log = logger;

                System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(@"~\");
                System.Configuration.ConnectionStringSettings connString = null;
                if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
                    connString = rootWebConfig.ConnectionStrings.ConnectionStrings["AppMetricsConnectionString"];
                else
                    throw new Exception("Invalid ConnectionString."); 

                if (connString != null)
                {
                    var oDB = new AppMetrics.Common.DataBase();
                    oDB.ConnectionString = connString.ConnectionString;

                    //Set database information
                    bizServer.DataBase = oDB;
                }

                Application.Add("BIZSERVER", bizServer);
                Application.Add("RNGCSP", new System.Security.Cryptography.RNGCryptoServiceProvider());
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.TraceError(ex.StackTrace);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Current.Response.Cache.SetNoStore();

            EnableCrossDmainAjaxCall();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            try
            {
                if (Application["BIZSERVER"] != null)
                {
                    var bizServer = (AppMetrics.Common.BizServer)Application["BIZSERVER"];

                    #region Remove cached items

                    AppMetrics.Common.CacheHelper cache = new AppMetrics.Common.CacheHelper();
                    cache.Cache().Dispose();

                    #endregion

                    bizServer.Log.TraceLog("Application_End");

                    bizServer.Log.CloseLog();
                    bizServer.Log = null;
                    bizServer = null;
                }
            }
            catch { }
        }

        private void EnableCrossDmainAjaxCall()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
    }
}