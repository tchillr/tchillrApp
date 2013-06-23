using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using System.ServiceModel.Channels;

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITchillrWCFService" in both code and config file together.
    [ServiceContract]
    public interface ITchillrWCFService
    {
        #region GET

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "DBActivities")]
        TchillrREST.DataModel.TchillrResponse GetFromDBAllActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "{theme}/Tags")]
        TchillrREST.DataModel.TchillrResponse GetTags(string theme);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        TchillrREST.DataModel.TchillrResponse GetInterests(string usernameid);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "activities/timespan/{nbDays}")]
        TchillrREST.DataModel.TchillrResponse GetActivitiesForDays(string nbDays);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "Themes")]
        TchillrREST.DataModel.TchillrResponse GetThemes();

        //[OperationContract]
        //[WebInvoke(Method = "GET",
        //    ResponseFormat = WebMessageFormat.Json,
        //    BodyStyle = WebMessageBodyStyle.Wrapped,
        //          UriTemplate = "DBCategories")]
        //string GetCategories();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/activities")]
        TchillrREST.DataModel.TchillrResponse GetUserActivities(string usernameid);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/activities/timespan/{nbDays}")]
        TchillrREST.DataModel.TchillrResponse GetUserActivitiesForDays(string usernameid, string nbDays);

        #endregion

        #region POST

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        TchillrREST.DataModel.TchillrResponse PostInterests(string usernameid, Stream content);

        #endregion
    }
}
