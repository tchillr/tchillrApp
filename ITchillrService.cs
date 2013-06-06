using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITchillrService" in both code and config file together.
    [ServiceContract]
    public interface ITchillrService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "StaticActivities")]
        List<Data.Activity> GetStaticAllActivities();

    }
}
