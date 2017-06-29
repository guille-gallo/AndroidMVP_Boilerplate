using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppMetrics.Logic.Model
{
    public class CallData
    {
        public string workStationCode { get; set; }
        public string phoneNumber { get; set; }
        public int secuenceNumber { get; set; }
        public string strDate { get; set; }
        public DateTime date { get; set; }
        public bool isNew { get; set; }
        public int crc { get; set; }
    }
}
