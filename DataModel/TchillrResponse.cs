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

        private string _data;
        public string data { get { return _data; } set { _data = value; } }

        public void SetData(object jsonObject){
            data = JsonConvert.SerializeObject(jsonObject, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() });
        }
    }
}