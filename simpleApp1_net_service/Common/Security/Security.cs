using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AppMetrics.Common.Utils;
using AppMetrics.Logger;
using System.Security;
using System.Web;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;

using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace AppMetrics.Common.Security
{
  public class Security
  {
	  public static byte[] Ping(BizServer oBizServer, XmlElement objParams)
	  {
		  return new byte[] { (byte)'O', (byte)'K' };
	  }

	  public static bool TrustAllCertificateCallback(object sender,
		X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
	  {
		  //Return True to force the certificate to be accepted.
		  return true;
	  }

  }

}
