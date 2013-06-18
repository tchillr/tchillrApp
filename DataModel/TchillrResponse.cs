using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TchillrREST.DataModel
{
    public class TchillrResponse
    {
        public bool success { get; set; }
        public double responseTime { get; set; }

        private object _data;
        public object data { get { return _data; } set { _data = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() }); } }
    }
}