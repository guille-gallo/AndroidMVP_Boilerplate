using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using WCFAppMetrics.ServiceResponses;

namespace WCFAppMetrics.MobileResponses
{
    [DataContract]
    public class TaxiGuardianResult : BaseResponse
    {
        List<TaxiGuardian> _items;


        public TaxiGuardianResult() 
        {
            _items = new List<TaxiGuardian>();
        }

        [DataMember]
        public List<TaxiGuardian> items
        {
            get { return _items; }
            set { _items = value; }
        }

        public class TaxiGuardian
        {
            public string usersOnlineQty { get; set; }
            public string usersNewQty { get; set; }            
        }
    }
}