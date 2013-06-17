using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Tchillr" in code, svc and config file together.
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Tchillr : ITchillr
    {
        //public List<TchillrREST.DataModel.Activity> GetUserActivities(string usernameid)
        //{
            
        //    //context.Configuration.ProxyCreationEnabled = false;

        //    List<Activity> lstActi = context.Activities.ToList<Activity>();
        //    List<Tag> lstTags = context.Tags.ToList<Tag>();

        //    List<int> userTags = GetInterests(usernameid);
        //    List<string> userContextualTags = new List<string>();
        //    foreach (Tag tag in lstTags.Where(tg => userTags.Contains(tg.ID)))
        //    {
        //        userContextualTags.Add(tag.Title);
        //        tag.WordsCloud = context.WordsCloud.Where(twc => twc.TagID == tag.ID).ToList<WordCloud>();
        //        foreach (WordCloud wc in tag.WordsCloud)
        //        {
        //            userContextualTags.Add(wc.Title);
        //        }
        //    }
        //    //List<string> userContextualTags = context.Tags.Where(tg => userTags.Contains(tg.ID)).Select(x => x.Title).ToList<string>();

        //    userContextualTags = userContextualTags.ConvertAll(d => d.ToUpper());

        //    //List<string> tags = context.Tags.Select(tg => tg.Title).ToList<string>();
        //    //tags = tags.ConvertAll(d => d.ToUpper());

        //    foreach (Activity act in lstActi)
        //    {
        //        act.Keywords = context.Keywords.Where(keywords => keywords.ActivityID == act.ID).ToList<Keyword>();
        //        //act.Keywords = act.GetKeywords(tags);
        //        act.ActivityContextualTags = act.GetContextualTags(userContextualTags);
        //        act.Occurences = context.Occurences.Where(os => os.ActivityID == act.ID).ToList<Occurence>();
        //        //act.Occurences = (from c in context.Occurences select c).Take(1).ToList<Occurence>();
        //    }

        //    return lstActi.Where(acti => acti.ActivityContextualTags.Intersect(userContextualTags).Count() > 0).OrderByDescending(acti => acti.ActivityContextualTags.Intersect(userContextualTags).Count()).ToList<Data.Activity>();

        //}

        public string GetFromDBAllActivities()
        {
            TchillrREST.DataModel.TchillrDataBaseEntities context = new TchillrREST.DataModel.TchillrDataBaseEntities(ConfigurationManager.ConnectionStrings["TchillrDataBaseEntities"].ConnectionString);
            context.ContextOptions.ProxyCreationEnabled = false;
            List<TchillrREST.DataModel.Activity> lstActivities = context.Activities.ToList();
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //string json = js.Serialize(lstActivities);


            return JsonConvert.SerializeObject(lstActivities);
        }
    }
}
