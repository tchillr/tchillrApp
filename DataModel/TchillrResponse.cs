using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Net;

namespace TchillrREST.DataModel
{
    public class TchillrResponse
    {
        public bool success { get; set; }
        public double responseTime { get; set; }

        [JsonIgnore]
        public DateTime start { get; set; }

        private object _data;
        public object data { get { return _data; } set { _data = value; } }

        public TchillrResponse()
        {
            start = DateTime.Now;
        }

        public virtual void SetData(object jsonObject)
        {
            //_data = new  List<object>();
            //_data.Add(JsonConvert.SerializeObject(jsonObject, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() }));
            _data = jsonObject;
            //_data = JsonConvert.SerializeObject(jsonObject, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() });
        }

        public Message GetResponseMessage()
        {
            responseTime = (DateTime.Now - this.start).TotalMilliseconds;

            string myResponseBody = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() });

            //TchillrREST.Utilities.TchillrContext.Connection.Close();
            if (success)
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
            else
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
            return WebOperationContext.Current.CreateTextResponse(myResponseBody,
                        "application/json; charset=utf-8",
                        Encoding.UTF8);
        }

    }

}