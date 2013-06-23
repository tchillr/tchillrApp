﻿using System;
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

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TchillrWCFService" in code, svc and config file together.
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TchillrWCFService : ITchillrWCFService
    {
        #region GET

        public TchillrREST.DataModel.TchillrResponse GetFromDBAllActivities()
        {
            DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Activities);
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetTags(string theme)
        {
            DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Tags.Where(tg => tg.Theme.title == theme));
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetInterests(string usernameid)
        {
            DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            int userNameID = int.Parse(usernameid);

            tchill.SetData(TchillrREST.Utilities.TchillrContext.UserTags.Where(user => user.identifier == userNameID));
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetActivitiesForDays(string nbDays)
        {
            DateTime start = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

            DateTime till = DateTime.Now.AddDays(double.Parse(nbDays));
            var activitiesForDays = from acti in TchillrREST.Utilities.TchillrContext.Activities
                                    from occ in TchillrREST.Utilities.TchillrContext.Occurences
                                    where acti.identifier == occ.ActivityID && occ.jour > DateTime.Now && occ.jour < till
                                    select acti;

            tchill.SetData(activitiesForDays);
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - start).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetThemes()
        {
            DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Themes);
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetDBCategories()
        {
            DateTime now = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();
            tchill.SetData(TchillrREST.Utilities.TchillrContext.Categories);
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - now).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetUserActivities(string usernameid)
        {
            DateTime start = DateTime.Now;
            TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

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

            tchill.SetData(userActivities.OrderByDescending(acti => acti.score).ToList());
            tchill.success = true;
            tchill.responseTime = (DateTime.Now - start).TotalMilliseconds;
            return tchill;
        }

        public TchillrREST.DataModel.TchillrResponse GetUserActivitiesForDays(string usernameid, string nbDays)
        {
            try
            {
                DateTime start = DateTime.Now;
                TchillrREST.DataModel.TchillrResponse tchill = new DataModel.TchillrResponse();

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

                tchill.SetData(userActivities.OrderByDescending(acti => acti.score).ToList<DataModel.Activity>());
                tchill.success = true;
                tchill.responseTime = (DateTime.Now - start).TotalMilliseconds;
                return tchill;
                
                /*string myResponseBody = JsonConvert.SerializeObject(tchill, Formatting.None, new JsonSerializerSettings { ContractResolver = new TchillrREST.Contract.ContractResolver() });
                return WebOperationContext.Current.CreateTextResponse(myResponseBody,
                            "application/json; charset=utf-8",
                            Encoding.UTF8);
                 */
            }
            catch (Exception exp) { }
            return null;
            //return JsonConvert.SerializeObject(.ToList());
        }

        #endregion

        #region POST

        public TchillrREST.DataModel.TchillrResponse PostInterests(string usernameid, Stream content)
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
