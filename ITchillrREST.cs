using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using TchillrREST.Data;
using System.ServiceModel.Web;

namespace TchillrREST
{
    [ServiceContract]
    public interface ITchillrREST
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
                  ResponseFormat = WebMessageFormat.Json,
                  UriTemplate = "Activities")]
        List<Activity> GetAllActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
                  ResponseFormat = WebMessageFormat.Json,
                  UriTemplate = "StaticActivities")]
        string GetStaticAllActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
                  ResponseFormat = WebMessageFormat.Json,
                  UriTemplate = "VLibStations")]
        string GetAllVLibStations();
    }
}