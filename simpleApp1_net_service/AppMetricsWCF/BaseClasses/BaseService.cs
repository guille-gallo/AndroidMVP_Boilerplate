using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel;

using AppMetrics.Logic;
using AppMetrics.Common;
using AppMetrics.Logger;
using WCFAppMetrics.MobileResponses;

namespace WCFAppMetrics
{
    public class BaseService
    {
        private BizServer _bizServer = null;
        private RNGCryptoServiceProvider _rngCsp = null;
        private CacheHelper _cache = null;
        private HttpRequestMessageProperty _httpRequestMessageProperty = null;

        public BaseService()
        {
            _bizServer = GetBizServer((BizServer)HttpContext.Current.Application["BIZSERVER"], OperationContext.Current, this);
            _rngCsp = (RNGCryptoServiceProvider)HttpContext.Current.Application["RNGCSP"];
            _cache = new CacheHelper();
            _httpRequestMessageProperty = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
        }

        protected BizServer bizServer 
        {
            get { return _bizServer; }
        }

        protected Log logger
        {
            get { return _bizServer.Log; }
        }

        protected RNGCryptoServiceProvider rngCsp
        {
            get { return _rngCsp; }
        }

        protected CacheHelper cache
        {
            get { return _cache; }
        }

        protected bool IsUserValid(string sessionID)
        {
            if (sessionID == null)
                return false;

            return cache.Exists(GetCacheKey(sessionID));
        }

       /* protected bool IsUserGroupAdmin(LoginResult userInfo)
        {
            if (userInfo == null)
                return false;

            if (Enum.IsDefined(typeof(CoreConstants.UserGroupType), userInfo.userTypeID) && (CoreConstants.UserGroupType)userInfo.userTypeID == CoreConstants.UserGroupType.GroupAdministrator)
                return true;
            else
                return false;
        }*/

        protected void AddItemToCache<T>(string cacheKey, T cacheItem)
        {
            cacheKey = HttpUtility.UrlDecode(cacheKey);
            cache.Add<T>(cacheItem, cacheKey, new CacheItemPolicy() { Priority = CacheItemPriority.Default, SlidingExpiration = TimeSpan.FromMinutes(Parameter.Get<int>(_bizServer, Parameter.Keys.CacheSlidingExpiration)), RemovedCallback = ItemRemoved });
        }

        private void ItemRemoved(CacheEntryRemovedArguments arguments)
        {
            try
            {
                bizServer.Log.TraceLog("ItemRemoved (removed reason: " + arguments.RemovedReason.ToString() + ")");

                if (arguments.RemovedReason != CacheEntryRemovedReason.CacheSpecificEviction)
                {
                    bizServer.Log.TraceLog("Logging out user...");
                    //var removedObject = (LoginResult)arguments.CacheItem.Value;

                    using (AppMetrics.Logic.User user = new AppMetrics.Logic.User(bizServer))
                    {
                        if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
                        {
                            //user.Logout(removedObject.userID, HttpUtility.UrlEncode(arguments.CacheItem.Key));
                        }
                        else
                        {
                            //user.Logout(removedObject.userID, String.Empty);
                        }
                    }
                    //bizServer.Log.TraceLog("User (" + removedObject.userID.ToString() + ") logged out...");
                }
            }
            catch (Exception ex)
            {
                bizServer.Log.TraceError(ex.Message + " | " + ex.StackTrace);
            }
        }

        protected void RemoveItemFromCache(string cacheKey)
        {
            if (cacheKey != null)
                cache.Remove(GetCacheKey(cacheKey));
        }

        protected T GetItemFromCache<T>(string cacheKey) where T : class
        {
            if (cacheKey != null)
                return cache.Get<T>(GetCacheKey(cacheKey));
            else
                return null;
        }

        protected string GetSessionID(RNGCryptoServiceProvider rngCsp, int length)
        {
            Byte[] sessionID = new Byte[length];
            rngCsp.GetBytes(sessionID);
            return HttpUtility.UrlEncode(Convert.ToBase64String(sessionID));
        }

        protected void ProcessException<T>(Exception ex, string rawErrorMessage, ref T response) where T : WCFAppMetrics.ServiceResponses.BaseResponse
        {
            if (ex is ValidationException)
            {
                logger.TraceError(rawErrorMessage, bizServer.Usuario.RemoteEndpoint);
                response.status.status = (int)WCFAppMetrics.ServiceResponses.StatusResult.ValidationError;
                response.status.errorMessage = ex.Message;
            }
            else
            {
                if (ex is SessionExpiredException)
                {
                    logger.TraceError(rawErrorMessage, bizServer.Usuario.RemoteEndpoint);
                    response.status.status = (int)WCFAppMetrics.ServiceResponses.StatusResult.SessionExpired;
                    response.status.errorMessage = ex.Message;
                }
                else
                {
                    logger.TraceError(rawErrorMessage + " (" + ex.StackTrace + ")", bizServer.Usuario.RemoteEndpoint);
                    response.status.status = (int)WCFAppMetrics.ServiceResponses.StatusResult.GeneralError;
                }
            }
        }

        private BizServer GetBizServer(BizServer genericBizServer, OperationContext operationContext, BaseService baseService)
        {
            return new BizServer() { Log = genericBizServer.Log, DataBase = genericBizServer.DataBase, Usuario = new Usuario() { RemoteEndpoint = GetRemoteEndpoint(operationContext), IsMobile = (baseService is IMobile) } };
        }

        private string GetRemoteEndpoint(OperationContext operationContext)
        {
            try
            {
                RemoteEndpointMessageProperty endpointProperty = operationContext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return endpointProperty.Address + ":" + endpointProperty.Port.ToString();
            }
            catch { return String.Empty; }
        }

        private string GetCacheKey(string cacheKey)
        {
            return (_httpRequestMessageProperty.Method == "POST") ? HttpUtility.UrlDecode(cacheKey) : cacheKey;
        }
    }
}