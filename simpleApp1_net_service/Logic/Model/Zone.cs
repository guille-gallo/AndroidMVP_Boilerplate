using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppMetrics.Logic.Model
{
    public class Zone
    {
			public int id { get; set; }
			public string name { get; set; }
			public string color { get; set; }
			public int companyID { get; set; }
			public List<Model.Point> points { get; set; }
    }
}