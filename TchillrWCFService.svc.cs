using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Data.Objects.DataClasses;
using System.Web;
using System.Globalization;
using System.Data;
using log4net;

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TchillrWCFService" in code, svc and config file together.
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TchillrWCFService : ITchillrWCFService
    {
        #region Log4Net
        protected static readonly ILog log = LogManager.GetLogger(typeof(TchillrWCFService));
        #endregion

        #region TokenID

        /*
         bitar.alaa@gmail.com
         30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d
         */
        /*
         jad
         70f38523e129a2a9c0aa0a08f26b569fd060ba86e691b40342e501710688cac1
         */
        /*
         Tchillr@gmail.com
          05b1bc4403734e5a11db3be63c2792597bc1af3998790282d66eb39dfc073bee
         */
        /*
         alaa.bitar@orange.com
          70f38523e129a2a9c0aa0a08f26b569ffe0d64cc438386fd94b19652d92c67d840cafd2914e4bb2f4decbc1d566769e2
         */

        #endregion

        #region GET


        public Message GetFromDBAllActivities()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            //tchill.SetData(TchillrREST.Utilities.TchillrContext.Activities);
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Activities.Take(100).ToList<DataModel.Activity>());
            tchill.success = true;

            return tchill.GetResponseMessage();
        }

        public Message GetTags(string theme)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            //List<DataModel.Tag> tags = new List<DataModel.Tag>();

            //DataModel.Theme thm = TchillrREST.Utilities.TchillrContext.Themes.FirstOrDefault(th => th.title == theme);
            //if(thm != null){
            //    foreach (DataModel.Tag tag in thm.Tags)
            //        tags.Add(tag);
            //}

            //tchill.SetData(tags);

            tchill.SetData(TchillrREST.Utilities.TchillrContext.Tags.Where(tg => tg.Theme.title == theme).ToList<DataModel.Tag>());

            tchill.success = true;
            return tchill.GetResponseMessage();
        }

        public Message GetInterests(string usernameid)
        {
            log.Debug("Calling with " + usernameid);
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            try
            {
                Guid userNameID = Guid.Parse(usernameid);
                DataModel.User currenUser = TchillrREST.Utilities.TchillrContext.Users.FirstOrDefault(usr => usr.identifier == userNameID);

                List<DataModel.Tag> results = new List<DataModel.Tag>();

                if (currenUser != null)
                {
                    log.Debug("currentUser name" + currenUser.name);
                    results = currenUser.UserTags.Select(usrTags => usrTags.Tag).ToList();
                }
                else
                    log.Debug("username id does not exists");
                //foreach (DataModel.UserTag userTag in TchillrREST.Utilities.TchillrContext.UserTags.Where(user => user.UserID == userNameID))
                //{
                //    DataModel.Tag tag = TchillrREST.Utilities.TchillrContext.Tags.FirstOrDefault(tg => tg.identifier == userTag.TagID);
                //    if (tag != null)
                //        results.Add(tag);
                //}
                tchill.SetData(results);
                tchill.success = true;
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message + " stacktrace " + exp.StackTrace);
                tchill.success = false;
                tchill.SetData(exp.Message);
            }

            return tchill.GetResponseMessage();
        }

        public Message GetActivitiesForDays(string fromDate, string toDate)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            DateTime now;
            try
            {
                now = DateTime.ParseExact(fromDate, TchillrREST.Utilities.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
            }
            catch (FormatException frmExp)
            {
                tchill.success = false;
                tchill.data = "Could not parse " + fromDate + " ";
                return tchill.GetResponseMessage();
            }
            DateTime till;
            try
            {
                till = DateTime.ParseExact(toDate, TchillrREST.Utilities.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
            }
            catch (FormatException frmExp)
            {
                tchill.success = false;
                tchill.data = "Could not parse " + toDate + " ";
                return tchill.GetResponseMessage();
            }

            TimeSpan fromTime = now.TimeOfDay;
            TimeSpan tillTime = till.TimeOfDay;

            now = now.Date;
            till = till.Date;
            //var activitiesForDays = from acti in TchillrREST.Utilities.TchillrContext.Activities
            //                        from occ in TchillrREST.Utilities.TchillrContext.Occurences
            //                        where acti.identifier == occ.ActivityID && occ.jour >= now && occ.jour <= till
            //                        && acti.latitude > 0 && acti.longitude > 0
            //                        select acti;

            var activitiesForDays = TchillrREST.Utilities.TchillrContext.Activities.Where(act => act.Occurences.Count(oc => (oc.jour == now && fromTime >= oc.hour_start && fromTime <= oc.hour_end) || (oc.jour == now && fromTime <= oc.hour_start) || (oc.jour > now && oc.jour <= till)) > 0 &&
                                                                                                 act.latitude > 0 && act.longitude > 0
                                                                                          ).OrderBy(act => act.Occurences.FirstOrDefault().jour).ThenBy(act => act.Occurences.FirstOrDefault().hour_start);

            foreach (DataModel.Activity activity in activitiesForDays)
            {

                List<DataModel.Keyword> keywords = activity.Keywords.ToList();
                List<string> keywordsString = keywords.Select(keyword => keyword.title).ToList().ConvertAll(d => d.ToUpper());
                List<string> rubirquesString = activity.Rubriques.Select(rub => rub.name).ToList().ConvertAll(d => d.ToUpper());

                activity.tags = new List<DataModel.ContextualTag>();

                var activityTags = from dbTags in TchillrREST.Utilities.TchillrContext.Tags
                                   where keywordsString.Contains(dbTags.title.ToUpper()) ||
                                   dbTags.WordClouds.FirstOrDefault(wd => keywordsString.Contains(wd.title.ToUpper())) != null ||
                                   rubirquesString.Contains(dbTags.title.ToUpper()) || dbTags.WordClouds.FirstOrDefault(wd => rubirquesString.Contains(wd.title.ToUpper())) != null
                                   select new { dbTags.identifier, dbTags.title, dbTags.themeID };

                foreach (var element in activityTags)
                {
                    DataModel.ContextualTag ct = new DataModel.ContextualTag();
                    ct.identifier = element.identifier;
                    ct.title = element.title;
                    ct.themeID = element.themeID.Value;
                    activity.tags.Add(ct);
                }

                // identify the color of the activity
                if (string.IsNullOrEmpty(activity.color))
                {
                    if (activity.tags.Count > 0)
                    {
                        DataModel.ContextualTag ct = activity.tags.FirstOrDefault();
                        if (ct != null)
                            activity.color = TchillrREST.Utilities.TchillrContext.Tags.First(tg => tg.identifier == ct.identifier).Theme.title;
                    }
                }

                activity.OccurencesToSend = activity.Occurences.Where(oc => (oc.jour == now && fromTime >= oc.hour_start && fromTime <= oc.hour_end) || (oc.jour == now && fromTime <= oc.hour_start) || (oc.jour > now && oc.jour <= till)).ToList<DataModel.Occurence>();
            }

            try
            {
                tchill.SetData(activitiesForDays.ToList<DataModel.Activity>());
                tchill.success = true;
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.data = exp.Message;
                tchill.success = false;
            }
            return tchill.GetResponseMessage();
        }

        public Message GetThemes()
        {
            log.Debug("Calling");
            //DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Themes.ToList<DataModel.Theme>());
            tchill.success = true;
            //tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill.GetResponseMessage();
        }

        public Message GetDBCategories()
        {
            //DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Categories.ToList<DataModel.Category>());
            tchill.success = true;
            //tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill.GetResponseMessage();
        }

        public Message GetUserActivities(string usernameid)
        {
            //DateTime start = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            Guid userNameID = Guid.Parse(usernameid);
            List<DataModel.Activity> userActivities = new List<DataModel.Activity>();

            List<string> tags = new List<string>();
            List<string> tagWordsCloud = new List<string>();

            List<int> userTags = TchillrREST.Utilities.TchillrContext.UserTags.Where(user => user.UserID == userNameID).Select(userTag => userTag.TagID).ToList();
            foreach (DataModel.Tag tag in TchillrREST.Utilities.TchillrContext.Tags)
                if (userTags.Contains(tag.identifier))
                    tags.Add(tag.title);

            tagWordsCloud = TchillrREST.Utilities.TchillrContext.WordClouds.Where(wordCloud => userTags.Contains(wordCloud.tagID)).Select(wordCloud => wordCloud.title).ToList();

            tags = tags.ConvertAll(d => d.ToUpper());
            tagWordsCloud = tagWordsCloud.ConvertAll(d => d.ToUpper());

            foreach (DataModel.Activity activity in TchillrREST.Utilities.TchillrContext.Activities)
            {

                List<string> keywordsString = activity.Keywords.Select(keyword => keyword.title).ToList();

                var activityTags = from dbTags in TchillrREST.Utilities.TchillrContext.Tags
                                   where keywordsString.Contains(dbTags.title) || dbTags.WordClouds.FirstOrDefault(wd => keywordsString.Contains(wd.title.ToUpper())) != null
                                   select new { dbTags.identifier, dbTags.title };

                activity.tags = new List<DataModel.ContextualTag>();
                foreach (var element in activityTags)
                {
                    DataModel.ContextualTag ct = new DataModel.ContextualTag();
                    ct.identifier = element.identifier;
                    ct.title = element.title;
                    activity.tags.Add(ct);
                }
                //activity.tags = activityTags.ToDictionary(grp => grp.identifier, grp => grp.title);

                activity.score = 0;
                foreach (DataModel.Keyword keyword in activity.Keywords)
                {
                    string upperTitle = keyword.title.ToUpper();
                    if (tags.Contains(upperTitle))
                        activity.score += keyword.hits;
                    if (tagWordsCloud.Contains(upperTitle))
                        activity.score += 1;
                }

                foreach (DataModel.Rubrique rubrique in activity.Rubriques)
                {
                    string upperName = rubrique.name.ToUpper();
                    if (tags.Contains(upperName))
                        activity.score += Utilities.RUBRIQUE_WEIGHT;
                    if (tagWordsCloud.Contains(upperName))
                        activity.score += 1;
                }

                if (activity.score > 0)
                {
                    if (!userActivities.Contains(activity))
                        userActivities.Add(activity);
                }
            }

            tchill.SetData(userActivities.OrderByDescending(acti => acti.score).ToList());
            tchill.success = true;
            //tchill.responseTime = (DateTime.Now - start).TotalMilliseconds;
            return tchill.GetResponseMessage();
        }

        public Message GetUserActivitiesForDays(string usernameid, string fromDate, string toDate)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            try
            {

                Guid userNameID = Guid.Empty;
                Guid.TryParse(usernameid, out userNameID);

                List<DataModel.Activity> userActivities = new List<DataModel.Activity>();

                List<string> tags = new List<string>();
                List<string> tagWordsCloud = new List<string>();

                DataModel.User currenUser = TchillrREST.Utilities.TchillrContext.Users.FirstOrDefault(usr => usr.identifier == userNameID);

                if (currenUser == null)
                    return GetActivitiesForDays(fromDate, toDate);

                tags = currenUser.UserTags.Select(usrTags => usrTags.Tag.title).ToList();

                if (tags.Count == 0)
                {
                    return GetActivitiesForDays(fromDate, toDate);
                }

                List<int> userTags = currenUser.UserTags.Select(userTag => userTag.TagID).ToList();

                DateTime now;
                try
                {
                    now = DateTime.ParseExact(fromDate, TchillrREST.Utilities.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                }
                catch (FormatException frmExp)
                {
                    tchill.success = false;
                    tchill.data = "Could not parse " + fromDate + " ";
                    return tchill.GetResponseMessage();
                }
                DateTime till;
                try
                {
                    till = DateTime.ParseExact(toDate, TchillrREST.Utilities.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                }
                catch (FormatException frmExp)
                {
                    tchill.success = false;
                    tchill.data = "Could not parse " + toDate + " ";
                    return tchill.GetResponseMessage();
                }

                tagWordsCloud = TchillrREST.Utilities.TchillrContext.WordClouds.Where(wordCloud => userTags.Contains(wordCloud.tagID)).Select(wordCloud => wordCloud.title).ToList();

                tags = tags.ConvertAll(d => d.ToUpper());
                tagWordsCloud = tagWordsCloud.ConvertAll(d => d.ToUpper());

                TimeSpan fromTime = now.TimeOfDay;
                TimeSpan tillTime = till.TimeOfDay;

                now = now.Date;
                till = till.Date;
                //var activitiesForDays = from acti in TchillrREST.Utilities.TchillrContext.Activities
                //                        from occ in TchillrREST.Utilities.TchillrContext.Occurences
                //                        where acti.identifier == occ.ActivityID && occ.jour >= now && occ.jour <= till
                //                        && acti.latitude > 0 && acti.longitude > 0
                //                        select acti;
                var activitiesForDays = TchillrREST.Utilities.TchillrContext.Activities.Where(act => act.Occurences.Count(oc => (oc.jour == now && fromTime >= oc.hour_start && fromTime <= oc.hour_end) || (oc.jour == now && fromTime <= oc.hour_start) || (oc.jour > now && oc.jour <= till)) > 0 &&
                                                                                                 act.latitude > 0 && act.longitude > 0
                                                                                          );

                foreach (DataModel.Activity activity in activitiesForDays)
                {
                    //activitiesScrore[activity.identifier] = 0;
                    activity.color = string.Empty;

                    List<DataModel.Keyword> keywords = activity.Keywords.ToList();
                    List<string> keywordsString = keywords.Select(keyword => keyword.title).ToList().ConvertAll(d => d.ToUpper());
                    List<string> rubirquesString = activity.Rubriques.Select(rub => rub.name).ToList().ConvertAll(d => d.ToUpper());

                    var activityTags = from dbTags in TchillrREST.Utilities.TchillrContext.Tags
                                       where keywordsString.Contains(dbTags.title.ToUpper()) ||
                                       dbTags.WordClouds.FirstOrDefault(wd => keywordsString.Contains(wd.title.ToUpper())) != null ||
                                       rubirquesString.Contains(dbTags.title.ToUpper()) || dbTags.WordClouds.FirstOrDefault(wd => rubirquesString.Contains(wd.title.ToUpper())) != null
                                       select new { dbTags.identifier, dbTags.title, dbTags.themeID };

                    activity.OccurencesToSend = activity.Occurences.Where(oc => (oc.jour == now && fromTime >= oc.hour_start && fromTime <= oc.hour_end) || (oc.jour == now && fromTime <= oc.hour_start) || (oc.jour > now && oc.jour <= till)).ToList<DataModel.Occurence>();

                    activity.tags = new List<DataModel.ContextualTag>();
                    foreach (var element in activityTags)
                    {
                        DataModel.ContextualTag ct = new DataModel.ContextualTag();
                        ct.identifier = element.identifier;
                        ct.title = element.title;
                        ct.themeID = element.themeID.Value;
                        activity.tags.Add(ct);
                        if (string.IsNullOrEmpty(activity.color) && userTags.Contains(element.identifier))
                            activity.color = TchillrREST.Utilities.TchillrContext.Tags.First(tg => tg.identifier == element.identifier).Theme.title;
                    }

                    // identify the color of the activity
                    if (string.IsNullOrEmpty(activity.color))
                    {
                        if (activity.tags.Count > 0)
                        {
                            DataModel.ContextualTag ct = activity.tags.FirstOrDefault();
                            if (ct != null)
                                activity.color = TchillrREST.Utilities.TchillrContext.Tags.First(tg => tg.identifier == ct.identifier).Theme.title;
                        }
                    }

                    activity.score = 0;
                    foreach (DataModel.Keyword keyword in keywords)
                    {
                        string upperTitle = keyword.title.ToUpper();
                        if (tags.Contains(upperTitle))
                            //activitiesScrore[activity.identifier] += keyword.hits;
                            activity.score += keyword.hits;
                        if (tagWordsCloud.Contains(upperTitle))
                            //activitiesScrore[activity.identifier] += 1;
                            activity.score += 1;
                    }

                    foreach (DataModel.Rubrique rubrique in activity.Rubriques)
                    {
                        string upperName = rubrique.name.ToUpper();
                        if (tags.Contains(upperName))
                            //activitiesScrore[activity.identifier] += Utilities.RUBRIQUE_WEIGHT;
                            activity.score += Utilities.RUBRIQUE_WEIGHT;
                        if (tagWordsCloud.Contains(upperName))
                            //activitiesScrore[activity.identifier] += 1;
                            activity.score += 1;
                    }

                    if (activity.score > 0)
                    {
                        if (!userActivities.Contains(activity))
                            userActivities.Add(activity);
                    }
                }

                tchill.SetData(userActivities.OrderByDescending(acti => acti.score).ToList<DataModel.Activity>());
                tchill.success = true;
            }
            catch (System.InvalidOperationException invOpExp)
            {
                return GetActivitiesForDays(fromDate, toDate);
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message + " stacktrace " + exp.StackTrace);
                tchill.success = false;
                tchill.data = exp.Message;
            }

            return tchill.GetResponseMessage();
        }

        public Message TestParisAPI(string offset, string limit)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            string BASE_URL = @"https://api.paris.fr:3000/data/1.1/QueFaire/get_activities/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d&created=0";
            try
            {
                WebRequest req = WebRequest.Create(BASE_URL + "&offset=" + offset + "&limit=" + limit);

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {

                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        tchill.data = reader.ReadToEnd();
                    }
                }

            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.success = false;
                tchill.data = exp.Message;
            }

            return tchill.GetResponseMessage();
        }

        public Message UpdateMedia(string skip)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            int numberOfItemToSkip = int.Parse(skip);
            foreach (DataModel.Activity activity in TchillrREST.Utilities.TchillrContext.Activities.Where(act => act.Media.Count == 0).OrderBy(act => act.identifier).Skip(numberOfItemToSkip).ToList())
            {
                WebRequest req = WebRequest.Create(@"https://api.paris.fr:3000/data/1.0/QueFaire/get_activity/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d&id=" + activity.identifier);
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                using (Stream respStream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                    JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
                    foreach (JObject jsonActivity in jsonActivities["data"])
                    {
                        foreach (JObject media in jsonActivity["media"])
                        {
                            string path = media["path"].ToString();
                            DataModel.Medium medium = activity.Media.FirstOrDefault(med => med.path == path);
                            if (medium == null)
                            {
                                medium = new DataModel.Medium();
                                medium.caption = string.IsNullOrEmpty(media["caption"].ToString()) ? "" : media["caption"].ToString();
                                medium.credit = media["credit"].ToString();
                                medium.path = media["path"].ToString();
                                medium.type = media["type"].ToString();
                                activity.Media.Add(medium);
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                }
            }

            TchillrREST.Utilities.TchillrContext.SaveChanges();

            tchill.success = true;
            tchill.data = "done";

            return tchill.GetResponseMessage();
        }


        const string HTML_TAG_PATTERN = @"<[^>]*>";
        const string BASE_URL = @"https://api.paris.fr:3000/data/1.1/QueFaire/get_activities/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d&created=0";
        //const string LIMIT = "58";

        static string StripHTML(string inputString)
        {
            return Regex.Replace
              (inputString, HTML_TAG_PATTERN, string.Empty);
        }

        public Message InjectToDataBase(string off, string lmt)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            DateTime yesturday = DateTime.Now.AddDays(-1);
            int counter = 1;
            try
            {
                DataModel.Entities context = TchillrREST.Utilities.TchillrContext;
                int offset = int.Parse(off);
                int loopLimit = offset + 500;
                while (offset <= loopLimit)
                {
                    WebRequest req = WebRequest.Create(BASE_URL + "&offset=" + offset + "&limit=" + lmt);

                    req.Method = "GET";
                    try
                    {
                        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {

                            using (Stream respStream = resp.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(respStream, Encoding.UTF8);

                                JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
                                foreach (JObject activity in jsonActivities["data"])
                                {
                                    int ident = (int)activity["idactivites"];
                                    bool insert = false;
                                    DataModel.Activity act = context.Activities.FirstOrDefault(acti => acti.identifier == ident);
                                    if (act == null || act.identifier == 0)
                                    {
                                        act = new DataModel.Activity();
                                        act.Occurences = new EntityCollection<DataModel.Occurence>();
                                        act.Rubriques = new EntityCollection<DataModel.Rubrique>();
                                        insert = true;
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    EntityCollection<DataModel.Occurence> occurences = new EntityCollection<DataModel.Occurence>();
                                    foreach (JObject occ in activity["occurences"])
                                    {
                                        DataModel.Occurence occurence = new DataModel.Occurence();
                                        occurence.jour = occ["jour"] == null || occ["jour"].ToString() == string.Empty ? System.Data.SqlTypes.SqlDateTime.MinValue.Value : DateTime.Parse(occ["jour"].ToString());
                                        if (occurence.jour < yesturday)
                                            continue;

                                        TimeSpan tsTemp = TimeSpan.Zero;

                                        if (!TimeSpan.TryParse(occ["hour_start"].ToString(), out tsTemp) || tsTemp.TotalHours > 23 || tsTemp.TotalHours < 0)
                                            TimeSpan.TryParse("00:00:00", out tsTemp);

                                        occurence.hour_start = tsTemp;

                                        tsTemp = TimeSpan.Zero;

                                        if (!TimeSpan.TryParse(occ["hour_end"].ToString(), out tsTemp) || tsTemp.TotalHours > 23 || tsTemp.TotalHours < 0)
                                            TimeSpan.TryParse("23:59:59", out tsTemp);

                                        occurence.hour_end = tsTemp;

                                        //occurence.hour_start = TimeSpan.Parse(occ["hour_start"].ToString());
                                        //if (occurence.hour_start.TotalHours == 24)
                                        //    occurence.hour_start = TimeSpan.Parse("23:59:59");
                                        //occurence.hour_end = TimeSpan.Parse(occ["hour_end"].ToString());
                                        //if (occurence.hour_end.TotalHours == 24)
                                        //    occurence.hour_end = TimeSpan.Parse("23:59:59");

                                        if (occurences.Where(occu => occu.jour == occurence.jour && occu.hour_start == occurence.hour_start && occu.hour_end == occurence.hour_end).Count() == 0)
                                            occurences.Add(occurence);
                                    }


                                    foreach (DataModel.Occurence occ in occurences.OrderBy(occ => occ.jour).ThenBy(occ => occ.hour_start).ThenBy(occ => occ.hour_end))
                                        act.Occurences.Add(occ);

                                    // we do not add an activity in the database if it is already obsolete or if there is no Occurences
                                    if (act.Occurences.Count == 0 || (act.Occurences.Count > 0 && act.Occurences.Last().jour < yesturday))
                                        continue;

                                    act.name = WebUtility.HtmlDecode(activity["nom"].ToString());
                                    act.adress = WebUtility.HtmlDecode(activity["adresse"].ToString());
                                    act.city = WebUtility.HtmlDecode(activity["city"].ToString());
                                    act.description = StripHTML(WebUtility.HtmlDecode(activity["description"].ToString()));
                                    act.identifier = (int)activity["idactivites"];
                                    act.zipcode = (int)activity["zipcode"];
                                    act.place = activity["lieu"].ToString();
                                    act.accessType = activity["accessType"].ToString();
                                    act.hasFee = activity["hasFee"].ToString();
                                    act.created = activity["created"] == null || activity["created"].ToString() == string.Empty ? System.Data.SqlTypes.SqlDateTime.MinValue.Value : DateTime.Parse(activity["created"].ToString());

                                    act.shortDescription = string.Empty;

                                    float temp = 0;
                                    if (!string.IsNullOrEmpty(activity["lat"].ToString()))
                                    {
                                        try
                                        {
                                            temp = float.Parse(activity["lat"].ToString(), Utilities.FRENCH_CULTURE.NumberFormat);
                                        }
                                        catch (FormatException frmExp)
                                        {
                                            temp = float.Parse(activity["lat"].ToString(), Utilities.ENGLISH_CULTURE.NumberFormat);
                                        }
                                    }
                                    act.latitude = temp;
                                    temp = 0;
                                    if (!string.IsNullOrEmpty(activity["lat"].ToString()))
                                    {
                                        try
                                        {
                                            temp = float.Parse(activity["lon"].ToString(), Utilities.FRENCH_CULTURE.NumberFormat);
                                        }
                                        catch (FormatException frmExp)
                                        {
                                            temp = float.Parse(activity["lon"].ToString(), Utilities.ENGLISH_CULTURE.NumberFormat);
                                        }
                                    }
                                    act.longitude = temp;

                                    foreach (JObject rub in activity["rubriques"])
                                    {
                                        DataModel.Rubrique rubrique = new DataModel.Rubrique();
                                        rubrique.name = rub["rubrique"].ToString();
                                        act.Rubriques.Add(rubrique);
                                    }

                                    WebRequest req2 = WebRequest.Create(@"https://api.paris.fr:3000/data/1.0/QueFaire/get_activity/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d&id=" + activity["idactivites"]);
                                    HttpWebResponse resp2 = req2.GetResponse() as HttpWebResponse;
                                    using (Stream respStream2 = resp2.GetResponseStream())
                                    {
                                        StreamReader reader2 = new StreamReader(respStream2, Encoding.UTF8);
                                        JObject jsonActivities2 = JObject.Parse(reader2.ReadToEnd());
                                        foreach (JObject activity2 in jsonActivities2["data"])
                                        {
                                            act.shortDescription = StripHTML(HttpUtility.HtmlDecode(activity2["small_description"].ToString()));
                                            act.idorganisateurs = activity2["idorganisateurs"] == null ? 0 : (int)activity2["idorganisateurs"];
                                            act.idlieux = activity2["idlieux"] == null ? 0 : (int)activity2["idlieux"];
                                            act.updated = activity2["updated"] == null || activity2["updated"].ToString() == string.Empty ? System.Data.SqlTypes.SqlDateTime.MinValue.Value : DateTime.Parse(activity2["updated"].ToString());
                                            act.price = StripHTML(WebUtility.HtmlDecode(activity2["price"].ToString()));
                                            act.metro = StripHTML(WebUtility.HtmlDecode(activity2["metro"].ToString()));
                                            act.velib = StripHTML(WebUtility.HtmlDecode(activity2["velib"].ToString()));
                                            act.bus = StripHTML(WebUtility.HtmlDecode(activity2["bus"].ToString()));
                                            foreach (JObject media in activity2["media"])
                                            {
                                                DataModel.Medium medium = new DataModel.Medium();
                                                medium.caption = string.IsNullOrEmpty(media["caption"].ToString()) ? "" : media["caption"].ToString();
                                                medium.credit = media["credit"].ToString();
                                                medium.path = media["path"].ToString();
                                                medium.type = media["type"].ToString();
                                                act.Media.Add(medium);
                                            }
                                        }
                                    }

                                    TchillrREST.Utilities.SetKeywords(act);

                                    Console.WriteLine("Adding acitivity # " + counter + " with id " + act.identifier);
                                    if (insert)
                                        context.Activities.AddObject(act);
                                    counter++;
                                }
                            }
                        }

                        offset += int.Parse(lmt);
                    }
                    catch (Exception exp)
                    {
                        offset += int.Parse(lmt);
                    }
                }

                context.SaveChanges();
                tchill.success = true;
                tchill.data = "done added " + counter + " activities";

            }
            catch (Exception exp)
            {
                tchill.success = false;
                tchill.data = exp.Message;
                if (exp.InnerException != null)
                    tchill.data += " " + exp.InnerException.Message;

                log.Error("Exp message " + tchill.data);
            }

            return tchill.GetResponseMessage();
        }

        public Message cleanDataBase()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            List<int> idsObsoletActivities = new List<int>();
            List<DataModel.Activity> obsoleteActivities = new List<DataModel.Activity>();
            try
            {
                DateTime yesturday = DateTime.Now.AddDays(-1);
                foreach (DataModel.Activity acti in TchillrREST.Utilities.TchillrContext.Activities)
                {
                    if (acti.Occurences.Count > 0 && acti.Occurences.Last().jour < yesturday)
                    {
                        idsObsoletActivities.Add(acti.identifier);
                        obsoleteActivities.Add(acti);
                    }

                }

                foreach (DataModel.Activity obsActi in obsoleteActivities)
                {
                    TchillrREST.Utilities.TchillrContext.Activities.DeleteObject(obsActi);
                }

                TchillrREST.Utilities.TchillrContext.SaveChanges();

                tchill.success = true;
                tchill.SetData(idsObsoletActivities);
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.success = false;
                tchill.SetData(exp.Message);
            }
            return tchill.GetResponseMessage();

        }

        public Message cleanOccurences()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            List<DataModel.Occurence> obsoleteOccurences = new List<DataModel.Occurence>();

            try
            {
                DateTime yesturday = DateTime.Now.AddDays(-1);

                obsoleteOccurences = TchillrREST.Utilities.TchillrContext.Occurences.Where(occ => occ.jour < yesturday).ToList();

                foreach (DataModel.Occurence occ in obsoleteOccurences)
                    TchillrREST.Utilities.TchillrContext.Occurences.DeleteObject(occ);

                TchillrREST.Utilities.TchillrContext.SaveChanges();

                tchill.success = true;
                tchill.data = obsoleteOccurences.Count;
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.success = false;
                tchill.data = exp.Message;
            }

            return tchill.GetResponseMessage();
        }

        public Message fixActivity(string activityID)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            int actId = int.Parse(activityID);

            foreach (DataModel.Activity act in TchillrREST.Utilities.TchillrContext.Activities.Where(act => act.shortDescription.StartsWith("\n")))
            {
                try
                {
                    act.shortDescription = act.shortDescription;
                }
                catch (Exception exp)
                {
                    log.Error("Exp message " + exp.Message);
                }
            }

            TchillrREST.Utilities.TchillrContext.SaveChanges();

            tchill.success = true;
            tchill.data = "done";
            return tchill.GetResponseMessage();
        }

        public Message fixKeywords()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            try
            {
                foreach (DataModel.Activity act in TchillrREST.Utilities.TchillrContext.Activities)
                {
                    TchillrREST.Utilities.SetKeywords(act);
                }

                TchillrREST.Utilities.TchillrContext.SaveChanges();

                tchill.data = "done";
                tchill.success = true;
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.data = exp.Message;
                tchill.success = false;
            }


            return tchill.GetResponseMessage();
        }

        public Message fixLatLon()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            foreach (DataModel.Activity act in TchillrREST.Utilities.TchillrContext.Activities.Where(act => act.longitude > 3))
            {
                try
                {
                    act.longitude = double.Parse(act.longitude.ToString().Insert(1, ","));

                    act.latitude = double.Parse(act.latitude.ToString().Insert(2, ","));
                }
                catch (Exception exp)
                {
                    log.Error("Exp message " + exp.Message);
                }
            }

            TchillrREST.Utilities.TchillrContext.SaveChanges();

            tchill.success = true;
            tchill.data = "done";
            return tchill.GetResponseMessage();
        }

        public Message fixCharactersInActivities()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            try
            {
                foreach (DataModel.Activity act in TchillrREST.Utilities.TchillrContext.Activities)
                {
                    if (act.name.Contains("&"))
                    {
                        act.name = System.Web.HttpUtility.HtmlDecode(act.name);
                    }

                    if (act.description.Contains("&"))
                    {
                        act.description = System.Web.HttpUtility.HtmlDecode(act.description);
                    }

                    if (act.shortDescription.Contains("&"))
                    {
                        act.shortDescription = System.Web.HttpUtility.HtmlDecode(act.shortDescription);
                    }

                    if (act.place.Contains("&"))
                    {
                        act.place = System.Web.HttpUtility.HtmlDecode(act.place);
                    }

                    if (act.adress.Contains("&"))
                    {
                        act.adress = System.Web.HttpUtility.HtmlDecode(act.adress);
                    }

                    if (act.city.Contains("&"))
                    {
                        act.city = System.Web.HttpUtility.HtmlDecode(act.city);
                    }

                    if (act.accessType.Contains("&"))
                    {
                        act.accessType = System.Web.HttpUtility.HtmlDecode(act.accessType);
                    }

                    if (act.price.Contains("&"))
                    {
                        act.price = System.Web.HttpUtility.HtmlDecode(act.price);
                    }

                    if (act.metro.Contains("&"))
                    {
                        act.metro = System.Web.HttpUtility.HtmlDecode(act.metro);
                    }

                    if (act.velib.Contains("&"))
                    {
                        act.velib = System.Web.HttpUtility.HtmlDecode(act.velib);
                    }

                    if (act.bus.Contains("&"))
                    {
                        act.bus = System.Web.HttpUtility.HtmlDecode(act.bus);
                    }

                    if (act.organisateur != null && act.organisateur.Contains("&"))
                    {
                        act.organisateur = System.Web.HttpUtility.HtmlDecode(act.organisateur);
                    }

                    if (act.hasFee.Contains("&"))
                    {
                        act.hasFee = System.Web.HttpUtility.HtmlDecode(act.hasFee);
                    }
                }

                TchillrREST.Utilities.TchillrContext.SaveChanges();

                tchill.success = true;
                tchill.data = "done";
            }

            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.success = false;
                tchill.data = exp.Message;
            }

            return tchill.GetResponseMessage();
        }

        public Message fixDescription()
        {

            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            try
            {
                foreach (DataModel.Activity act in TchillrREST.Utilities.TchillrContext.Activities.Where(act => act.name == act.description))
                {
                    WebRequest req2 = WebRequest.Create(@"https://api.paris.fr:3000/data/1.0/QueFaire/get_activity/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d&id=" + act.identifier);
                    HttpWebResponse resp2 = req2.GetResponse() as HttpWebResponse;
                    using (Stream respStream2 = resp2.GetResponseStream())
                    {
                        StreamReader reader2 = new StreamReader(respStream2, Encoding.UTF8);
                        JObject jsonActivities2 = JObject.Parse(reader2.ReadToEnd());
                        foreach (JObject activity2 in jsonActivities2["data"])
                        {
                            act.description = StripHTML(WebUtility.HtmlDecode(activity2["description"].ToString()));
                        }
                    }
                }
                TchillrREST.Utilities.TchillrContext.SaveChanges();

                tchill.data = "done";
                tchill.success = true;
            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.success = false;
                tchill.data = exp.Message;
            }

            return tchill.GetResponseMessage();
        }


        //UriTemplate = "users/{usernameid}/activities/{activityID}/went")]
        public Message UserActivityDontLike(string usernameID, string activityID)
        {
            return UserActivity(usernameID, activityID, 1);
        }

        public Message UserActivityGoing(string usernameID, string activityID)
        {
            return UserActivity(usernameID, activityID, 2);
        }

        public Message UserActivityAttending(string usernameID, string activityID)
        {
            return UserActivity(usernameID, activityID, 3);
        }

        private Message UserActivity(string usernameID, string activityID, int status)
        {
            int uID = int.Parse(usernameID);
            int actID = int.Parse(activityID);

            DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            DataModel.UserActivity userActi = TchillrREST.Utilities.TchillrContext.UserActivities.FirstOrDefault(ua => ua.activityID == actID && ua.userID == uID);
            if (userActi != null)
            {
                userActi.status = status;
            }
            else
            {
                userActi = new DataModel.UserActivity();
                userActi.activityID = actID;
                DataModel.Activity act = TchillrREST.Utilities.TchillrContext.Activities.FirstOrDefault(acti => acti.identifier == actID);
                if (act != null)
                {
                    userActi.keywords = String.Join("###", act.Keywords.Select(key => key.title).ToList());
                }
                userActi.status = status;
                userActi.userID = uID;
                TchillrREST.Utilities.TchillrContext.UserActivities.AddObject(userActi);
            }

            TchillrREST.Utilities.TchillrContext.SaveChanges();

            return tchill.GetResponseMessage();
        }

        public Message AddUser(string userGUID)
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            try
            {
                Guid usrGuid = Guid.Empty;
                if (Guid.TryParse(userGUID, out usrGuid))
                {
                    DataModel.User newUser = TchillrREST.Utilities.TchillrContext.Users.FirstOrDefault(usr => usr.identifier == usrGuid);
                    if (newUser == null)
                    {
                        newUser = new DataModel.User();
                        newUser.identifier = usrGuid;
                        newUser.name = "User " + (TchillrREST.Utilities.TchillrContext.Users.Count() + 1);
                        TchillrREST.Utilities.TchillrContext.Users.AddObject(newUser);
                        TchillrREST.Utilities.TchillrContext.SaveChanges();
                        tchill.data = "user identifier:" + newUser.identifier + " name:" + newUser.name + " created.";
                        tchill.success = true;
                    }
                    else
                    {
                        tchill.data = "userGUID " + userGUID + " already exists.";
                        tchill.success = false;
                    }
                }
                else
                {
                    tchill.data = "userGUID " + userGUID + " could not be parse as a valid Guid.";
                    tchill.success = false;
                }

            }
            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                tchill.success = false;
                tchill.data = exp.Message;
            }
            return tchill.GetResponseMessage();
        }

        public Message Fault()
        {
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            tchill.success = false;

            tchill.data = "faulted response";

            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
            string myResponseBody = JsonConvert.SerializeObject(tchill, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() });
            return WebOperationContext.Current.CreateTextResponse(myResponseBody,
                        "application/json; charset=utf-8",
                        Encoding.UTF8);
        }

        #endregion

        #region POST

        public Message PostInterests(string usernameid, Stream content)
        {

            Guid userNameID;
            Guid.TryParse(usernameid, out userNameID);
            // convert Stream Data to StreamReader
            StreamReader reader = new StreamReader(content);

            string result = reader.ReadToEnd();

            foreach (string tagLine in result.Split('&'))
            {
                int tagID = int.Parse(tagLine.Split('=')[1]);

                TchillrREST.DataModel.UserTag ut = TchillrREST.Utilities.TchillrContext.UserTags.FirstOrDefault(userTag => userTag.TagID == tagID && userTag.UserID == userNameID);
                if (ut == null || ut.UserID == Guid.Empty)
                {
                    ut = new TchillrREST.DataModel.UserTag();
                    ut.UserID = userNameID;
                    ut.TagID = tagID;
                    TchillrREST.Utilities.TchillrContext.UserTags.AddObject(ut);
                }
                else
                    TchillrREST.Utilities.TchillrContext.UserTags.DeleteObject(ut);

                TchillrREST.Utilities.TchillrContext.SaveChanges();

            }
            return GetInterests(usernameid);

        }

        #endregion

        #region PUT

        public Message PutInterests(string usernameid, Stream content)
        {
            try
            {
                Guid userNameID;
                List<int> sentTagIDs = new List<int>();
                Guid.TryParse(usernameid, out userNameID);

                StreamReader reader = new StreamReader(content);

                string result = reader.ReadToEnd();

                foreach (string tagLine in result.Split('&'))
                {
                    int tagID = int.Parse(tagLine.Split('=')[1]);
                    if (!sentTagIDs.Contains(tagID))
                        sentTagIDs.Add(tagID);
                }

                List<DataModel.UserTag> userTags = TchillrREST.Utilities.TchillrContext.UserTags.Where(userTag => userTag.UserID == userNameID).ToList();

                foreach (int tgID in sentTagIDs)
                {
                    DataModel.UserTag ut = userTags.FirstOrDefault(userTag => userTag.identifier == tgID);
                    if (ut == null || ut.UserID == Guid.Empty)
                    {
                        ut = new TchillrREST.DataModel.UserTag();
                        ut.UserID = userNameID;
                        ut.TagID = tgID;
                        TchillrREST.Utilities.TchillrContext.UserTags.AddObject(ut);
                    }
                }

                foreach (DataModel.UserTag userTag in userTags.Where(userTag => !sentTagIDs.Contains(userTag.identifier)))
                {
                    TchillrREST.Utilities.TchillrContext.UserTags.DeleteObject(userTag);
                }

                TchillrREST.Utilities.TchillrContext.SaveChanges();
            }

            catch (Exception exp)
            {
                log.Error("Exp message " + exp.Message);
                TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
                tchill.data = exp.Message;
                tchill.success = false;
            }

            return GetInterests(usernameid);

        }

        #endregion
    }
}
