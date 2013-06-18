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

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TchillrWCFService" in code, svc and config file together.
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TchillrWCFService : ITchillrWCFService
    {
        #region GET

        public string GetFromDBAllActivities()
        {
            TchillrREST.Utilities.TchillrContext.ContextOptions.ProxyCreationEnabled = false;
            return JsonConvert.SerializeObject(TchillrREST.Utilities.TchillrContext.Activities);
        }

        public string GetTags(string theme)
        {
            TchillrREST.Utilities.TchillrContext.ContextOptions.ProxyCreationEnabled = false;

            return JsonConvert.SerializeObject(TchillrREST.Utilities.TchillrContext.Tags.Where(tg => tg.Theme.title == theme));
        }

        public string GetInterests(string usernameid)
        {
            int userNameID = int.Parse(usernameid);

            return JsonConvert.SerializeObject(TchillrREST.Utilities.TchillrContext.UserTags.Where(user => user.identifier == userNameID));
        }

        public string GetActivitiesForDays(string nbDays)
        {

            DateTime till = DateTime.Now.AddDays(double.Parse(nbDays));
            var activitiesForDays = from acti in TchillrREST.Utilities.TchillrContext.Activities
                                    from occ in TchillrREST.Utilities.TchillrContext.Occurences
                                    where acti.identifier == occ.ActivityID && occ.jour > DateTime.Now && occ.jour < till
                                    select acti;

            return JsonConvert.SerializeObject(activitiesForDays.ToList());
        }

        public string GetThemes()
        {
            
            return JsonConvert.SerializeObject(TchillrREST.Utilities.TchillrContext.Themes, Formatting.None);
        }

        public string GetDBCategories()
        {
            return JsonConvert.SerializeObject(TchillrREST.Utilities.TchillrContext.Categories);
        }

        public string GetUserActivities(string usernameid)
        {
            int userNameID = int.Parse(usernameid);
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
                                   where keywordsString.Contains(dbTags.title)
                                   select dbTags;

                activity.tags = activityTags.ToList();

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

            return JsonConvert.SerializeObject(userActivities.OrderByDescending(acti => acti.score).ToList());
        }

        public string GetUserActivitiesForDays(string usernameid, string nbDays)
        {
            int userNameID = int.Parse(usernameid);
            List<DataModel.Activity> userActivities = new List<DataModel.Activity>();
            //Dictionary<int, int> activitiesScrore = new Dictionary<int, int>();

            List<string> tags = new List<string>();
            List<string> tagWordsCloud = new List<string>();

            List<int> userTags = TchillrREST.Utilities.TchillrContext.UserTags.Where(user => user.UserID == userNameID).Select(userTag => userTag.TagID).ToList();
            foreach (DataModel.Tag tag in TchillrREST.Utilities.TchillrContext.Tags)
                if (userTags.Contains(tag.identifier))
                    tags.Add(tag.title);

            tagWordsCloud = TchillrREST.Utilities.TchillrContext.WordClouds.Where(wordCloud => userTags.Contains(wordCloud.tagID)).Select(wordCloud => wordCloud.title).ToList();

            tags = tags.ConvertAll(d => d.ToUpper());
            tagWordsCloud = tagWordsCloud.ConvertAll(d => d.ToUpper());

            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);

            DateTime till = now.AddDays(double.Parse(nbDays));
            var activitiesForDays = from acti in TchillrREST.Utilities.TchillrContext.Activities
                                    from occ in TchillrREST.Utilities.TchillrContext.Occurences
                                    where acti.identifier == occ.ActivityID && occ.jour >= now && occ.jour <= till
                                    select acti;

            foreach (DataModel.Activity activity in activitiesForDays)
            {
                //activitiesScrore[activity.identifier] = 0;

                List<string> keywordsString = activity.Keywords.Select(keyword => keyword.title).ToList();

                var activityTags = from dbTags in TchillrREST.Utilities.TchillrContext.Tags
                                   where keywordsString.Contains(dbTags.title)
                                   select dbTags;

                activity.tags = activityTags.ToList();

                activity.score = 0;
                foreach (DataModel.Keyword keyword in activity.Keywords)
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

            //List<int> activitiesID = activitiesScrore.Where(item => item.Value > 0).Select(item => item.Key).ToList();

            //var activitiesForUser = from acti in TchillrREST.Utilities.TchillrContext.Activities
            //                        where activitiesID.Contains(acti.identifier)
            //                        select acti;

            //var activitiesForUser = from acti in userActivities
            //                        orderby descending acti.score
            //                        select acti;

            //activitiesScrore.OrderBy(item => item.Value);

            return JsonConvert.SerializeObject(userActivities.OrderByDescending(acti => acti.score).ToList());
        }

        #endregion

        #region POST

        public string PostInterests(string usernameid, Stream content)
        {

            int userNameID = int.Parse(usernameid);
            // convert Stream Data to StreamReader
            StreamReader reader = new StreamReader(content);

            string result = reader.ReadToEnd();

            int tagID = int.Parse(result.Split('=')[1]);

            TchillrREST.DataModel.UserTag ut = TchillrREST.Utilities.TchillrContext.UserTags.FirstOrDefault(userTag => userTag.TagID == tagID && userTag.UserID == userNameID);
            if (ut == null || ut.UserID == 0)
            {
                ut = new TchillrREST.DataModel.UserTag();
                ut.UserID = userNameID;
                ut.TagID = tagID;
                TchillrREST.Utilities.TchillrContext.UserTags.AddObject(ut);
            }
            else
                TchillrREST.Utilities.TchillrContext.UserTags.DeleteObject(ut);

            TchillrREST.Utilities.TchillrContext.SaveChanges();


            return GetInterests(usernameid);

        }

        #endregion

    }
}
