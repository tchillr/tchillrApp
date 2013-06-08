using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using TchillrREST.Data;
using System.IO;

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
                  UriTemplate = "DBActivities")]
        List<Data.Activity> GetFromDBAllActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "{theme}/Tags")]
        List<Tag> GetTags(string theme);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        List<int> GetInterests(string usernameid);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        bool PostInterests(string usernameid, Stream content);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "Themes")]
        List<Theme> GetThemes();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "StaticActivities")]
        List<Data.Activity> GetStaticAllActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "StaticCategories")]
        List<Data.Categorie> GetStaticCategories();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "Categories")]
        List<Data.Categorie> GetCategories();

        [OperationContract]
        [WebInvoke(Method = "GET",
                  ResponseFormat = WebMessageFormat.Json,
                  UriTemplate = "Activities")]
        List<Activity> GetAllActivities();

    }
}
