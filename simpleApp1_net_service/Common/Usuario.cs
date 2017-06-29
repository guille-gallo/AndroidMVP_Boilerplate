using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using AppMetrics.Common;
using AppMetrics.Logger;

namespace AppMetrics.Common
{
    public class BizServer
    {
        public Log Log { get; set; }
        public Usuario Usuario { get; set; }
        public DataBase DataBase { get; set; }
    }

    public class Usuario
    {
        public string RemoteEndpoint { get; set; }
        public bool IsMobile { get; set; }
    }

    public class ParameterEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string Descripcion { get; set; }
    }

    public class DataBase
    {
        public string ConnectionString { get; set; }
    }
}
