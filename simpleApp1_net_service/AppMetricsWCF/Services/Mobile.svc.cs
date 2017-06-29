using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using System.Data;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using AppMetrics.Logic.Model;
using AppMetrics.Logic.Utils;
using AppMetrics.Common;
using AppMetrics.Logger;
using AppMetrics.Common.Utils;
using WCFAppMetrics.ServiceResponses;
using WCFAppMetrics.MobileResponses;
using AppMetrics.WCFAppMetrics.Resources;
using AppMetrics.Logic;

namespace WCFAppMetrics
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Mobile : BaseService, IMobile
    {
        private string TAG_startLog = "|************************************ Start: ";
        private string TAG_finishLog = "|************************************ Finish: ";
        private string TAG_asterisks = " ************************************|";

        /// <summary>
        /// Get online passenger user's type.
        /// </summary>        
        /// <returns>Status and online users list</returns>
        public TaxiGuardianResult GetCardInfo()
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetCardInfo" + TAG_asterisks);
            var response = new TaxiGuardianResult();

            try
            {
                DataTable items;
                using (AppMetrics.Logic.TaxiGuardian objeto = new AppMetrics.Logic.TaxiGuardian(bizServer))
                {
                    object result = new DataTable();
                    objeto.GetCardInfo(ref result);
                    items = (DataTable)result;
                }

                foreach (DataRow row in items.Rows)
                {
                    var item = new TaxiGuardianResult.TaxiGuardian();
                    item.usersOnlineQty = row["UsersOnlineQty"].ToString();
                    item.usersNewQty = row["UsersNewQty"].ToString();
                    response.items.Add(item);
                }
                response.status.status = (int)StatusResult.OK;

            }
            catch (Exception ex)
            {
                ProcessException<TaxiGuardianResult>(ex, Errors.GetMessagesByUserFormat(ex.Message), ref response);
            }

            bizServer.Log.TraceLog(TAG_finishLog + "GetCardInfo" + TAG_asterisks);
            return response;
        }

        /// <summary>
        /// Get online passenger user's type.
        /// </summary>        
        /// <returns>Status and online users list</returns>
       /* public TaxiGuardianResult GetOnlinePassengerUsers()
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetOnlinePassengerUsers" + TAG_asterisks);
            var response = new TaxiGuardianResult();

            try
            {
                    DataTable items;
                    using (AppMetrics.Logic.TaxiGuardian objeto = new AppMetrics.Logic.TaxiGuardian(bizServer))
                    {
                        object result = new DataTable();
                        objeto.GetOnlinePassengerUsers(ref result);
                        items = (DataTable)result;
                    }

                    foreach (DataRow row in items.Rows)
                    {
                        var item = new TaxiGuardianResult.TaxiGuardian();
                        item.userName = row["userName"].ToString();
                        item.userFirstName = row["FirstName"].ToString();
                        item.userLastName = row["LastName"].ToString();

                        response.items.Add(item);
                    }

                    response.status.status = (int)StatusResult.OK;
               
            }
            catch (Exception ex)
            {
                ProcessException<TaxiGuardianResult>(ex, Errors.GetMessagesByUserFormat(ex.Message), ref response);
            }

            bizServer.Log.TraceLog(TAG_finishLog + "GetOnlinePassengerUsers" + TAG_asterisks);
            return response;
        }*/

        /// <summary>
        /// Get offline passenger user's type.
        /// </summary>        
        /// <returns>Status and offline users list</returns>
        /*public TaxiGuardianResult GetOfflinePassengerUsers()
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetOfflinePassengerUsers" + TAG_asterisks);
            var response = new TaxiGuardianResult();

            try
            {
                DataTable items;
                using (AppMetrics.Logic.TaxiGuardian objeto = new AppMetrics.Logic.TaxiGuardian(bizServer))
                {
                    object result = new DataTable();
                    objeto.GetOfflinePassengerUsers(ref result);
                    items = (DataTable)result;
                }

                foreach (DataRow row in items.Rows)
                {
                    var item = new TaxiGuardianResult.TaxiGuardian();
                    item.userName = row["userName"].ToString();
                    item.userFirstName = row["FirstName"].ToString();
                    item.userLastName = row["LastName"].ToString();

                    response.items.Add(item);
                }

                response.status.status = (int)StatusResult.OK;

            }
            catch (Exception ex)
            {
                ProcessException<TaxiGuardianResult>(ex, Errors.GetMessagesByUserFormat(ex.Message), ref response);
            }

            bizServer.Log.TraceLog(TAG_finishLog + "GetOfflinePassengerUsers" + TAG_asterisks);
            return response;
        }

        /// <summary>
        /// Get recent created users (any type)
        /// </summary>        
        /// <returns>Status and recently created users list</returns>
        public TaxiGuardianResult GetRecentCreatedUsers()
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetRecentCreatedUsers" + TAG_asterisks);
            var response = new TaxiGuardianResult();

            try
            {
                DataTable items;
                using (AppMetrics.Logic.TaxiGuardian objeto = new AppMetrics.Logic.TaxiGuardian(bizServer))
                {
                    object result = new DataTable();
                    objeto.GetRecentCreatedUsers(ref result);
                    items = (DataTable)result;
                }

                foreach (DataRow row in items.Rows)
                {
                    var item = new TaxiGuardianResult.TaxiGuardian();
                    item.userName = row["userName"].ToString();
                    item.userFirstName = row["FirstName"].ToString();
                    item.userLastName = row["LastName"].ToString();

                    response.items.Add(item);
                }

                response.status.status = (int)StatusResult.OK;

            }
            catch (Exception ex)
            {
                ProcessException<TaxiGuardianResult>(ex, Errors.GetMessagesByUserFormat(ex.Message), ref response);
            }

            bizServer.Log.TraceLog(TAG_finishLog + "GetRecentCreatedUsers" + TAG_asterisks);
            return response;
        }*/

    }
}