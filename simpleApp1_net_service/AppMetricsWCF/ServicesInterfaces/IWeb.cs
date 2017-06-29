using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using WCFAppMetrics.ServiceResponses;
//using WCFAppMetrics.WebResponses;

namespace WCFAppMetrics
{
    [ServiceContract]
    public interface IWeb
    {
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        BaseResponse SetIVRStatus(string data, int secuenceNumber, string key);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<AppMetrics.Logic.Model.CallData> GetIVRStatus();

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        AppMetrics.Logic.Model.CallData GetIVRWorkstationCall(string workStationID);

    }
}
