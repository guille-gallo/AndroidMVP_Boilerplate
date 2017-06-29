using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Data.SqlTypes;
using AppMetrics.Data.ADONETDataAccess;
using AppMetrics.Logic;
using AppMetrics.Logic.BaseClass;
using AppMetrics.Logic.Utils;
using AppMetrics.Common;
using AppMetrics.Common.Security;
using AppMetrics.Logic.Resources;

namespace AppMetrics.Logic
{
  public class User : BaseClass.BaseClass
  {
    public User(BizServer bizSvr)
      : base(bizSvr, Assembly.GetCallingAssembly().GetName().Name, true)
    {
    }

    public User(BizServer bizSvr, bool createDBConnection)
      : base(bizSvr, Assembly.GetCallingAssembly().GetName().Name, createDBConnection)
    {
    }

    public User(BizServer bizSvr, DataAccess datAcc)
      : base(bizSvr, datAcc)
    {
    }
    
  }
}

