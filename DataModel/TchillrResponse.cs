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
        public object data { get { return _data; } set { _data = value;} }

        public virtual void SetData(object jsonObject)
        {
             //_data = new  List<object>();
             //_data.Add(JsonConvert.SerializeObject(jsonObject, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() }));
            _data = jsonObject;
            //_data = JsonConvert.SerializeObject(jsonObject, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() });
        }
         
    }

}