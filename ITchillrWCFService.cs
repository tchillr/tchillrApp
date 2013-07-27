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
        Message GetFromDBAllActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "{theme}/Tags")]
        Message GetTags(string theme);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        Message GetInterests(string usernameid);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/attendance")]
        Message GetUserAttendance(string usernameid);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "activities/from/{from}/to/{to}")]
        Message GetActivitiesForDays(string from, string to);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "activities/from/{from}/to/{to}/{tagids}")]
        Message GetActivitiesForTagsDays(string from, string to, string tagids);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "Themes")]
        Message GetThemes();

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
        Message GetUserActivities(string usernameid);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/activities/from/{from}/to/{to}")]
        Message GetUserActivitiesForDays(string usernameid, string from, string to);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "parisapi/{offset}/{limit}")]
        Message TestParisAPI(string offset, string limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "updateMedia/{skip}")]
        Message UpdateMedia(string skip);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "InjectToDataBase/{offset}/{limit}")]
        Message InjectToDataBase(string offset, string limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "cleanDataBase")]
        Message cleanDataBase();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "cleanOccs")]
        Message cleanOccurences();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "fixLatLon")]
        Message fixLatLon();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "fixActivity/{activityID}")]
        Message fixActivity(string activityID);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "fixKeywords")]
        Message fixKeywords();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "fixDesc")]
        Message fixDescription();
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "fixActivities")]
        Message fixCharactersInActivities();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/activities/{activityID}/maybe")]
        Message UserActivityMaybe(string usernameID, string activityID);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/activities/{activityID}/no")]
        Message UserActivityNo(string usernameID, string activityID);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/activities/{activityID}/yes")]
        Message UserActivityYes(string usernameID, string activityID);

        //[OperationContract]
        //[WebInvoke(Method = "GET",
        //    ResponseFormat = WebMessageFormat.Json,
        //    BodyStyle = WebMessageBodyStyle.Wrapped,
        //          UriTemplate = "users/{usernameid}/activities/{activityID}/dontlike")]
        //Message UserActivityDontLike(string usernameID, string activityID);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "login/adduser/{userguid}")]
        Message AddUser(string userguid);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "fault")]
        Message Fault();

        #endregion

        #region POST

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        Message PostInterests(string usernameid, Stream content);

        #endregion

        #region PUT

        [OperationContract]
        [WebInvoke(Method = "PUT",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
                  UriTemplate = "users/{usernameid}/interests")]
        Message PutInterests(string usernameid, Stream content);

        #endregion
    }
}
