using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Caching;
using System.Reflection;
using System.Globalization;
using AppMetrics.Data.ADONETDataAccess;
using AppMetrics.Logic;
using AppMetrics.Logic.BaseClass;
using AppMetrics.Logic.Utils;
using AppMetrics.Common;

namespace AppMetrics.Logic
{
    public class Parameter : BaseClass.BaseClass
    {
        #region Parameters Keys

        public struct Keys
        {
            public const string SMTPHost = "SMTP_HOST";
            public const string SMTPPort = "SMTP_PORT";
            public const string UploadImagesTemporaryDirectory = "UPLOAD_IMAGES_TEMP_PATH";
            public const string PublicURLUserProfilePicture = "PUBLIC_URL_USER_PROFILE_PICTURES";
            public const string MaxResultAllowed = "MAX_RESULT_ALLOWED";
            public const string AboutUSText = "ABOUT_US_TEXT";
            public const string PublicUrlTmpImages = "PUBLIC_URL_TMP_IMAGES";
            public const string MailFromConfirmationMail = "MAIL_FROM_CONFIRMATION_MAIL";
            public const string ConfirmationSignInPeriod = "CONFIRMATION_SIGNIN_PERIOD";
            public const string DefaultCity = "DEFAULT_CITY";
            public const string PublicUrlCompanyImages = "PUBLIC_URL_COMPANY_IMAGES";
            public const string UploadImagesCompanyDirectory = "UPLOAD_IMAGES_COMPANY_PATH";
            public const string TimeZoneOffset = "TIME_ZONE_OFFSET";
            public const string PushPenaltyAttempt = "PUSH_PENALTY_ATTEMPT";
            public const string PushSenderAuthToken = "PUSH_SENDER_AUTHTOKEN";
            public const string PushSenderAuthTokenPro = "PUSH_SENDER_AUTHTOKEN_PRO";
            public const string TimeToLivePushNotification = "TIME_TO_LIVE_PUSH_NOTIFICATION";
            public const string MaxPushEventsToProcess = "MAX_PUSH_EVENTS_TO_PROCESS";
            public const string DeletePushNotificationsAfterDays = "DELETE_PUSH_NOTIFICATIONS_AFTER_DAYS";
            public const string CacheSlidingExpiration = "CACHE_SLIDING_EXPIRATION";
            public const string SMTPUserName = "SMTP_USER_NAME";
            public const string SMTPPassword = "SMTP_PASSWORD";
            public const string RefreshParamsMobile = "REFRESH_PARAMS_MOBILE";
            public const string ProcessorTimeSamples = "PROCESSOR_TIME_SAMPLES";
            public const string LastVersionMobile = "LAST_VERSION_MOBILE";
            public const string LastVersionMobilePro = "LAST_VERSION_MOBILE_PRO";
            public const string MaxStorageForUsers = "MAX_STORAGE_FOR_USERS";
            public const string RadiusToSearchTaxi = "RADIUS_TO_SEARCH_TAXI";
            public const string TimeToShowTrip = "TIME_TO_SHOW_TRIP";
            public const string DistanceNearToTaxi = "DISTANCE_NEAR_TO_TAXI";
            public const string TimeToSetTaxiUsreDisconnected = "TIME_TO_SET_TAXIUSER_DISCONNECTED";
            public const string TimeToSearchTaxi = "TIME_TO_SEARCH_TAXI";
            public const string TimeToShowExpiredAlerts = "TIME_TO_SHOW_EXPIRED_ALERTS";
            public const string TermsAndConditions = "TERMS_AND_CONDITIONS";
            public const string MaxHeightImage = "MAX_HEIGHT_IMAGE";
            public const string MaxWidthImage = "MAX_WIDTH_IMAGE";
            public const string AvailableCities = "AVAILABLE_CITIES";
            public const string TimeToGetTripsMonitor = "TIME_TO_GET_TAXI_MONIOR";
            public const string AmountTripToAlert = "AMOUNT_TRIP_TO_ALERT";
            public const string TimeToAuditTrips = "TIME_TO_AUDIT_TRIPS";
            public const string TokenPrice = "TOKEN_PRICE";
            public const string TokenCountByKM = "TOKEN_COUNT_BY_KM";
            public const string ErrorRange = "ERROR_RANGE_TRIP_AMOUNT";
            public const string TokenCountFlagDrop = "TOKEN_COUNT_FLAG_DROP";
            public const string TokenPriceNormal = "TOKEN_PRICE_NORMAL";
            public const string PathToDBF = "PATH_TO_DBF";
            public const string PushSenderAuthTokenProIOS = "PUSH_SENDER_AUTHTOKEN_PRO_IOS";
        }

        #endregion

        public Parameter(BizServer bizSvr)
            : base(bizSvr, String.Empty, true) //String.Empty to avoid calling assembly validation to allow call this constructor from Get<T> method
        {
        }

        public Parameter(BizServer bizSvr, DataAccess datAcc)
            : base(bizSvr, datAcc)
        {
        }

        /// <summary>
        /// GetAll Parameters
        /// </summary>
        /// <param name="oResult"></param>
        public void GetAll(ref object oResult)
        {
            string strSQL = "";

            try
            {
                oDataAccess.ClearParameters();

                strSQL = "SELECT ParameterID, ParameterName, ParameterKey, ParameterValue, Description" +
                         " FROM Parameter WITH(NOLOCK)";

                CoreUtils.ExcecuteSQL(oDataAccess, strSQL, ref oResult);
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al obtener el listado de Parameters.");
            }
        }

        /// <summary>
        /// GetAll
        /// </summary>
        /// <param name="parameterID"></param>
        /// <param name="pageNum"></param>
        /// <param name="oResult"></param>
        /// <returns></returns>
        public int GetAll(int? parameterID, int? pageNum, ref object oResult)
        {
            string strSQL = "";

            try
            {
                oDataAccess.ClearParameters();
                oDataAccess.AddParameter("ParameterID", parameterID);
                oDataAccess.AddParameter("pageNum", pageNum);
                oDataAccess.AddOutputParameter("rowqty", SqlDbType.Int);

                strSQL = "spGetParameters";

                oResult = oDataAccess.GetDataTable(strSQL, CommandType.StoredProcedure);

                return oDataAccess.GetParameterValue<int>("rowqty");
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al obtener el listado de Parámetros.");
            }
        }

        /// <summary>
        /// GetMobile
        /// </summary>
        /// <param name="oResult"></param>
        public void GetMobile(ref object oResult)
        {
            string strSQL = "";

            try
            {
                oDataAccess.ClearParameters();
                oDataAccess.AddParameter("IsMobile", 1);

                strSQL = " SELECT * FROM [Parameter] WITH(NOLOCK)" +
                                 " WHERE IsMobile = ? ";

                oResult = oDataAccess.ExecuteQueryCache(strSQL, Get<int>(oBizServer, Parameter.Keys.RefreshParamsMobile), Parameter.Keys.RefreshParamsMobile, true);
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al obtener el listado de Parámetros Mobile.");
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="parameterID"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public int Update(int parameterID, string parameterValue, string description)
        {
            string strSQL = "";

            try
            {
                oDataAccess.ClearParameters();

                strSQL = "UPDATE Parameter SET ";

                oDataAccess.AddParameter("ParameterValue", parameterValue);
                strSQL += " ParameterValue=?,";

                oDataAccess.AddParameter("Description", description);
                strSQL += " Description=?";

                oDataAccess.AddParameter("ParameterID", parameterID);
                strSQL += " WHERE ParameterID=?";

                var rowsAffected = Convert.ToInt32(oDataAccess.ExecuteNonQuery(strSQL));

                if (rowsAffected != 1)
                {
                    throw new CoreException("No se ha podido actualizar el registro.", 1);
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al actualizar los datos del Parametro.");
            }
        }


        /// <summary>
        /// Add
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterKey"></param>
        /// <param name="parameterValue"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public int Add(string parameterName, string parameterKey, string parameterValue, string description, int isMobile)
        {
            string strSQL = "";
            int resultado;
            string strMsgValidate = "";

            try
            {
                strMsgValidate = ValidateParameterAdd(parameterKey);
                if (!string.IsNullOrEmpty(strMsgValidate))
                { // Si existen coincidencias
                    throw new CoreException(strMsgValidate, 1);
                }
                else
                {
                    oDataAccess.ClearParameters();
                    oDataAccess.AddParameter("ParameterName", parameterName);
                    oDataAccess.AddParameter("ParameterKey", parameterKey);
                    oDataAccess.AddParameter("ParameterValue", parameterValue);
                    oDataAccess.AddParameter("Description", description);
                    oDataAccess.AddParameter("IsMobile", isMobile);

                    strSQL = " INSERT INTO Parameter " +
                             " (ParameterName, ParameterKey, ParameterValue, Description, IsMobile) VALUES " +
                             " (?,?,?,?,?)";

                    resultado = Convert.ToInt32(oDataAccess.ExecuteIdentity(strSQL));

                    if (resultado == -1)
                    {
                        throw new CoreException("No se ha podido insertar el registro.", 1);
                    }

                    return resultado;
                }

            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al insertar el registro.");
            }
        }


        public string ValidateParameterAdd(string parameterKey)
        {
            string strSQL = "";

            oDataAccess.ClearParameters();
            oDataAccess.AddParameter("ParameterKey", parameterKey);

            strSQL = " SELECT * FROM [Parameter] " +
                     " WHERE ParameterKey = ? ";

            var dt = oDataAccess.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return "El 'Codigo' ya se encuentra en uso.";
            }

            return "";
        }


        /// <summary>
        /// SearchByName
        /// </summary>
        /// <param name="partialName"></param>
        /// <param name="maxResults"></param>
        /// <param name="oResult"></param>
        public void SearchByName(string partialName, int maxResults, ref object oResult)
        {
            string strSQL = "";

            try
            {

                if (maxResults > Parameter.Get<int>(oBizServer, Parameter.Keys.MaxResultAllowed))
                    throw new ValidationException(AppMetrics.Logic.Resources.Messages.InvalidParameter);

                oDataAccess.ClearParameters();
                oDataAccess.AddParameter("MaxResults", maxResults);
                oDataAccess.AddParameter("ParameterName", "%" + partialName + "%");


                strSQL = "SELECT TOP(?) ParameterID, ParameterName" +
                         " FROM Parameter WITH(NOLOCK)" +
                         " WHERE ParameterName LIKE ?";

                CoreUtils.ExcecuteSQL(oDataAccess, strSQL, ref oResult);
            }
            catch (Exception ex)
            {
                throw CustomException(ex, "Error inesperado al obtener el listado de Parámetros.");
            }
        }

        public static T Get<T>(BizServer bizServer, string key)
        {
            CacheHelper cache;
            ParameterEntity parameterItem;
            const string cacheKey = "PARAMETERS";

            try
            {
                cache = new CacheHelper();

                //Retrieve cached item
                var cacheParameters = cache.Get<Dictionary<string, ParameterEntity>>(cacheKey);

                if (cacheParameters == null)
                {
                    //Retrieve and add item to cache for next lookup
                    cacheParameters = new Dictionary<string, AppMetrics.Common.ParameterEntity>();

                    DataTable dtParameters;
                    using (Parameter parameter = new Parameter(bizServer))
                    {
                        object result = new DataTable();
                        parameter.GetAll(ref result);
                        dtParameters = (DataTable)result;
                    }

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var param = new ParameterEntity();
                        param.ID = Convert.ToInt32(row["ParameterID"]);
                        param.Name = row["ParameterName"].ToString();
                        param.Key = row["ParameterKey"].ToString();
                        param.Descripcion = row["Description"].ToString();

                        #region Parse Value According Type

                        var stringValue = row["ParameterValue"].ToString();
                        int parseInt;
                        DateTime parseDate;
                        Double parseDouble;

                        if (Int32.TryParse(stringValue, out parseInt))
                            param.Value = Convert.ToInt32(stringValue);
                        else
                        {
                            if (Double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out parseDouble))
                                param.Value = Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
                            else
                            {
                                if (DateTime.TryParse(stringValue, out parseDate))
                                    param.Value = Convert.ToDateTime(stringValue);
                                else
                                    param.Value = stringValue;
                            }
                        }

                        #endregion

                        cacheParameters.Add(param.Key, param);
                    }

                    int cacheAbsoluteExpirationTime = AppMetrics.Common.Utils.Utilities.ReadAppSettings<int>(System.Configuration.ConfigurationManager.AppSettings, "CacheAbsoluteExpirationTime", 60);
                    cache.Add<Dictionary<string, ParameterEntity>>(cacheParameters, cacheKey, new CacheItemPolicy() { Priority = CacheItemPriority.Default, AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheAbsoluteExpirationTime) });
                }

                //Return cached item
                if (cacheParameters.TryGetValue(key, out parameterItem))
                {
                    return (T)parameterItem.Value;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}