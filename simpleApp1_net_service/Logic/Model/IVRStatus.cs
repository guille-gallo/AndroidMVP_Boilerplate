using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppMetrics.Logic.Model
{
    public class IVRStatus
    {
        public List<CallData> data { get; set; }
        public string date { get; set; }
        public int secuenceNumber { get; set; }
    }
}
