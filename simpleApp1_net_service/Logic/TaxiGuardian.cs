using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Data.SqlTypes;    
using AppMetrics.Data.ADONETDataAccess;
using AppMetrics.Common;

namespace AppMetrics.Logic
{
    public class TaxiGuardian : BaseClass.BaseClass
    {

        public TaxiGuardian(BizServer bizSvr)
            : base(bizSvr, Assembly.GetCallingAssembly().GetName().Name, true)
        {
        }

        public TaxiGuardian(BizServer bizSvr, bool createDBConnection)
            : base(bizSvr, Assembly.GetCallingAssembly().GetName().Name, createDBConnection)
        {
        }

        public TaxiGuardian(BizServer bizSvr, DataAccess datAcc)
            : base(bizSvr, datAcc)
        {
        }

        public void GetCardInfo(ref object oResult)
        {
            string strSQL = "";
            string strSQL2 = "";
            string strSQL3 = "";
            
            try
            {
                /*
                 * 1) getonlinepassengerusers -> obtener row.count  LISTO
                 * 2) getrecentcreatedusers -> idem
                 * 3) insert en nueva tabla
                 * */
                oDataAccess.BeginTransaction();
                oDataAccess.ClearParameters();

                strSQL = "SELECT [User].* FROM [User] WITH(NOLOCK) " +
                        " WHERE UserTypeID = 5 " +
                        " AND UserStatusID = 1 ";
                DataTable users = oDataAccess.GetDataTable(strSQL);
                int onLineUsersQty = users.Rows.Count;

                strSQL2 = "SELECT [User].* FROM [User] WITH(NOLOCK) " +
                        " WHERE CreationDate > DateADD(mi, -30, GETDATE()) " +
                        " ORDER BY CreationDate DESC ";
                DataTable users2 = oDataAccess.GetDataTable(strSQL2);
                int recentNewUsers = users2.Rows.Count;

                oDataAccess.AddParameter("UsersOnlineQty", onLineUsersQty);
                oDataAccess.AddParameter("UsersNewQty", recentNewUsers);
                strSQL3 = " INSERT INTO AppMetricCard" +
                         " (UsersOnlineQty,UsersNewQty) VALUES " +
                         " (?,?)";

                /*
                 * 
                 * hacer select de lo insertado (strSQL3) y mandarlo en oResult
                 * 
                 */

                oResult = oDataAccess.GetDataTable(strSQL3);                
                oDataAccess.CommitTransaction();                

                /*oBizServer.Log.TraceLog("users.Rows.Count ->", (users.Rows.Count).ToString());
                oBizServer.Log.TraceLog("users ->", users.ToString());*/                
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado en GetCardInfo()");
            }
        }

        public void GetOnlinePassengerUsers(ref object oResult)
        {
            string strSQL = "";

            try
            {                
                oDataAccess.ClearParameters();

                strSQL = "SELECT [User].* FROM [User] WITH(NOLOCK) " +
                        " WHERE UserTypeID = 5 " +
                        " AND UserStatusID = 1 ";

                DataTable users = oDataAccess.GetDataTable(strSQL);

                oBizServer.Log.TraceLog("users.Rows.Count ->", (users.Rows.Count).ToString());
                oBizServer.Log.TraceLog("users ->", users.ToString());                

                oResult = users;
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado los pasajeros online.");
            }
        }

        public void GetOfflinePassengerUsers(ref object oResult)
        {
            string strSQL = "";

            try
            {
                oDataAccess.ClearParameters();

                strSQL = "SELECT [User].* FROM [User] WITH(NOLOCK) " +
                        " WHERE UserTypeID = 5 " +
                        " AND UserStatusID = 2 ";

                DataTable users = oDataAccess.GetDataTable(strSQL);                

                oResult = users;
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al obtener los pasajeros offline.");
            }
        }

        public void GetRecentCreatedUsers(ref object oResult)
        {
            string strSQL = "";

            try
            {
                oDataAccess.ClearParameters();

                strSQL = "SELECT [User].* FROM [User] WITH(NOLOCK) " +
                        " WHERE CreationDate > DateADD(mi, -30, GETDATE()) " +
                        " ORDER BY CreationDate DESC ";

                DataTable users = oDataAccess.GetDataTable(strSQL);

                oResult = users;
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al obtener los usuarios creados recientemente.");
            }
        }

    }
}