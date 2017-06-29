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
using AppMetrics.Logic;
using AppMetrics.Common;
using AppMetrics.Logger;
using AppMetrics.Common.Utils;
using WCFAppMetrics.ServiceResponses;
//using WCFAppMetrics.WebResponses;
using AppMetrics.WCFAppMetrics.Resources;
using AppMetrics.Logic.Model;
using AppMetrics.Logic.Utils;
using Newtonsoft.Json;

namespace WCFAppMetrics
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Web : BaseService, IWeb
    {

        private string TAG_startLog = "|************************************ Start: ";
        private string TAG_finishLog = "|************************************ Finish: ";
        private string TAG_asterisks = " ************************************|";

        private const string KEY = "R3spu3st4 OK";
        private byte[] desKey = new byte[16] { (byte)'f', (byte)'e', (byte)'i', (byte)'e', (byte)'-', (byte)'.', (byte)'f', (byte)'e', (byte)'d', (byte)' ', (byte)'5', (byte)'d', (byte)'2', (byte)'4', (byte)'m', (byte)'w' };

        static List<CallData> listIVRData = new List<CallData>();
        static int IVRDataSecuence;
        static DateTime IVRDate;

        public BaseResponse SetIVRStatus(string data, int secuenceNumber, string key)
        {
            bizServer.Log.TraceLog(TAG_startLog + "SetIVRStatus" + TAG_asterisks);
            var response = new BaseResponse();

            try
            {
                List<CallData> tempListIVRData = new List<CallData>();
                byte[] encryptedData = Convert.FromBase64String(key);
                TripleDes encrypt = new TripleDes(desKey, TripleDes.DES_DECRYPT);
                byte[] decryptedData = encrypt.des3_crypt_cbc(TripleDes.DES_DECRYPT, null, encryptedData);
                string decryotedKey = Encoding.ASCII.GetString(decryptedData);

                if (!decryotedKey.Contains(KEY))
                {
                    bizServer.Log.TraceLog(TAG_finishLog + "Error en Key" + TAG_asterisks);
                    response.status.status = (int)StatusResult.ValidationError;
                    return response;
                }
                if (secuenceNumber == 0 || secuenceNumber > IVRDataSecuence)
                {
                    IVRDate = DateTime.Now;
                    if (secuenceNumber > 0) IVRDataSecuence = secuenceNumber;
                    string[] dataLines = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < dataLines.Length; i += 2)
                    {
                        try
                        {
                            CallData callData = new CallData();
                            string[] args = dataLines[i].Split("=".ToCharArray());
                            callData.workStationCode = args[0];
                            callData.phoneNumber = args[1].Substring(args[1].LastIndexOf(")") + 1);
                            callData.phoneNumber = callData.phoneNumber.Trim();
                            //int index=0;
                            //for (; (index < callData.phoneNumber.Length) && (callData.phoneNumber.ToCharArray()[index] == ' '); index++) { }
                            //callData.phoneNumber = callData.phoneNumber.Substring(index);
                            callData.strDate = dataLines[i + 1].Substring(dataLines[i + 1].IndexOf("= ") + 2);
                            callData.date = DateTime.ParseExact(callData.strDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            callData.isNew = false;
                            CallData actualCallData = null;
                            foreach (CallData cd in listIVRData)
                            {
                                if (cd.workStationCode.Equals(callData.workStationCode))
                                {
                                    actualCallData = cd;
                                    break;
                                }
                            }
                            if (actualCallData == null)
                            {
                                callData.isNew = true;
                                //    tempListIVRData.Add(callData);
                            }
                            else if (callData.date.CompareTo(actualCallData.date) > 0)
                            {
                                callData.secuenceNumber = actualCallData.secuenceNumber+1;
                                callData.isNew = true;
                                //tempListIVRData.Add(callData);
                            }
                            else if (callData.phoneNumber != actualCallData.phoneNumber)
                            { 
                                callData.secuenceNumber = actualCallData.secuenceNumber+1;
                                callData.isNew = true;
                            }
                            else
                            {
                                callData.isNew = actualCallData.isNew;
                            }

                            tempListIVRData.Add(callData);
                        }
                        catch (Exception ex)
                        {
                            bizServer.Log.TraceLog(TAG_finishLog + "SetIVRStatus Item Error: " + ex.Message);
                        }
                    }
                    if (tempListIVRData.Count > 0)
                    {
                        listIVRData = tempListIVRData;
                    }
                    response.status.status = (int)StatusResult.OK;
                    AppMetrics.Logic.Utils.CoreUtils.saveCallData(tempListIVRData, @"D:\Apps\AppMetrics\Data\DataTmp", "IVRData.txt");
                }
                response.status.status = (int)StatusResult.OK;
            }
            catch (Exception ex)
            {
                BaseResponse resp = new BaseResponse();
                if (response.status.status != (int)StatusResult.OK)
                {
                    resp = response;
                }
                ProcessException<BaseResponse>(ex, Errors.LogoutFormat(ex.Message), ref resp);
                response.status.errorMessage = ex.Message;
            }

            bizServer.Log.TraceLog(TAG_finishLog + "SetIVRStatus" + TAG_asterisks);
            return response;
        }

        public List<AppMetrics.Logic.Model.CallData> GetIVRStatus()
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetIVRStatus" + TAG_asterisks);
            try
            {
                if (listIVRData == null || listIVRData.Count == 0)
                {
                    listIVRData = AppMetrics.Logic.Utils.CoreUtils.restoreCallData("", "IVRData.txt");
                }
            }
            catch (Exception ex)
            {

            }

            bizServer.Log.TraceLog(TAG_finishLog + "GetIVRStatus" + TAG_asterisks);
            return listIVRData;
        }

        public AppMetrics.Logic.Model.IVRStatus GetIVRFullStatus()
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetIVRFullStatus" + TAG_asterisks);
            IVRStatus data = new IVRStatus();
            try
            {
                if (listIVRData == null || listIVRData.Count == 0)
                {
                    listIVRData = AppMetrics.Logic.Utils.CoreUtils.restoreCallData("", "IVRData.txt");
                    IVRDate = DateTime.Now;
                }
                data.data = listIVRData;
                data.secuenceNumber = IVRDataSecuence;
                data.date = IVRDate.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {

            }

            bizServer.Log.TraceLog(TAG_finishLog + "GetIVRFullStatus" + TAG_asterisks);
            return data;
        }

        public AppMetrics.Logic.Model.CallData GetIVRWorkstationCall(string workStationID)
        {
            bizServer.Log.TraceLog(TAG_startLog + "GetIVRWorkstationCall" + TAG_asterisks);
            AppMetrics.Logic.Model.CallData callData = new AppMetrics.Logic.Model.CallData();

            try
            {
                if (listIVRData == null || listIVRData.Count == 0)
                {
                    listIVRData = AppMetrics.Logic.Utils.CoreUtils.restoreCallData("", "IVRData.txt");
                }
                foreach (CallData item in listIVRData)
                {
                    if (item.workStationCode.Equals(workStationID))
                    {
                        callData.workStationCode = item.workStationCode;
                        callData.phoneNumber = item.phoneNumber;
                        callData.strDate = item.strDate;
                        callData.date = item.date;
                        callData.secuenceNumber = item.secuenceNumber;
                        callData.isNew = item.isNew;
                        item.isNew = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            bizServer.Log.TraceLog(TAG_finishLog + "GetIVRWorkstationCall" + TAG_asterisks);
            return callData;
        }
    }
}
