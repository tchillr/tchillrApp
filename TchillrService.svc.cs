using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using System.ServiceModel.Activation;
using Newtonsoft.Json.Linq;
using TchillrREST.Data;
using System.Data.Objects;

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TchillrService" in code, svc and config file together.
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TchillrService : ITchillrService
    {
        const string HTML_TAG_PATTERN = @"<[^>]*>";

        static string StripHTML(string inputString)
        {
            return Regex.Replace
              (inputString, HTML_TAG_PATTERN, string.Empty);
        }

        public List<Data.Activity> GetStaticAllActivities()
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

            //List<Data.Activity> activities = new List<Data.Activity>();

            try
            {
                WebRequest req = WebRequest.Create("http://" + HttpContext.Current.Request.Url.Authority + "/staticActivy3.txt");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
                        foreach (JObject activity in jsonActivities["data"])
                        {
                            int identifier = (int)activity["identifier"];
                            Data.Activity act = context.Activities.Include("Occurences").FirstOrDefault(acti => acti.ID == identifier);
                            if (act == null)
                            {
                                act = new Activity();
                                act.Occurences = new List<Occurence>();
                                context.Activities.Add(act);
                            }
                            act.Nom = WebUtility.HtmlDecode(activity["name"].ToString());
                            act.Adresse = WebUtility.HtmlDecode(activity["adress"].ToString());
                            act.City = WebUtility.HtmlDecode(activity["city"] == null ? "" : activity["city"].ToString());
                            act.Description = StripHTML(WebUtility.HtmlDecode(activity["description"].ToString()));
                            act.ID = (int)activity["identifier"];
                            act.Zipcode = activity["zipcode"].ToString();
                            act.ShortDescription = activity["shortDescription"].ToString();
                            act.Lieu = activity["place"].ToString();

                            float temp = 0;
                            if (float.TryParse(activity["latitude"].ToString(), out temp))
                                act.Lat = temp;
                            temp = 0;
                            if (float.TryParse(activity["longitude"].ToString(), out temp))
                                act.Lon = temp;

                            foreach (JObject occ in activity["occurences"])
                            {
#warning convertir en start date end date

                                // we found that some activity have multiple equal occurences
                                if (act.Occurences.Exists(o => o.ActivityID == act.ID && o.Day == occ["jour"].ToString() && o.StartTime == occ["hour_start"].ToString() && o.EndTime == occ["hour_end"].ToString()))
                                    continue;
                                Occurence occurence = new Occurence();
                                occurence.Day = occ["jour"].ToString();
                                occurence.StartTime = occ["hour_start"].ToString();
                                occurence.EndTime = occ["hour_end"].ToString();
                                occurence.ActivityID = act.ID;
                                act.Occurences.Add(occurence);
                            }

                            //act.Keywords = act.GetKeywords();

                            //              activities.Add(act);

                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Data.Activity dumb = new Data.Activity();

                dumb.Adresse = exp.Message;
                dumb.City = exp.Source;
                dumb.Description = "dumb desc";
                dumb.ID = 1;

                //activities.Add(dumb);
            }

            context.SaveChanges();

            return context.Activities.ToList<Activity>();
            //return activities;
        }

        public List<Data.Activity> GetFromDBAllActivities()
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");
            context.Configuration.ProxyCreationEnabled = false;

            List<Activity> lstActi = context.Activities.ToList<Activity>();

            List<string> tags = context.Tags.Select(tg => tg.Title).ToList<string>();
            tags = tags.ConvertAll(d => d.ToUpper());

            foreach (Activity act in lstActi)
            {
                act.GetKeywords(tags);
                act.Occurences = context.Occurences.Where(os => os.ActivityID == act.ID).ToList<Occurence>();
            }

            return lstActi;
        }

        public List<Data.Activity> GetUserActivities(string usernameid)
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");
            context.Configuration.ProxyCreationEnabled = false;

            List<Activity> lstActi = context.Activities.ToList<Activity>();

            List<int> userTags = GetInterests(usernameid);
            List<string> userContextualTags = context.Tags.Where(tg => userTags.Contains(tg.ID)).Select(x => x.Title).ToList<string>();
            userContextualTags = userContextualTags.ConvertAll(d => d.ToUpper());

            List<string> tags = context.Tags.Select(tg => tg.Title).ToList<string>();
            tags = tags.ConvertAll(d => d.ToUpper());

            foreach (Activity act in lstActi)
            {
                act.GetKeywords(tags);
                act.Occurences = context.Occurences.Where(os => os.ActivityID == act.ID).ToList<Occurence>();
            }

            return lstActi.Where(acti => acti.ActivityContextualTags.Intersect(userContextualTags).Count() > 0).OrderByDescending(acti => acti.ActivityContextualTags.Intersect(userContextualTags).Count()).ToList<Data.Activity>();

        }

        public List<Data.Theme> GetThemes()
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

            context.Configuration.ProxyCreationEnabled = false;

            return context.Themes.Include("Tags").ToList<Data.Theme>();
        }

        public List<Data.Categorie> GetDBCategories()
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");
            return context.Categories.ToList<Categorie>();
        }

        public List<Data.Categorie> GetStaticCategories()
        {
            List<Data.Categorie> categories = new List<Categorie>();
            try
            {
                //TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

                WebRequest req = WebRequest.Create("http://" + HttpContext.Current.Request.Url.Authority + "/categories.txt");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
                        foreach (JObject categorie in jsonActivities["data"])
                        {
                            Categorie cat = new Categorie();
                            cat.Idcategorie = (int)categorie["idcategories"];
                            cat.Nom = WebUtility.HtmlDecode(categorie["name"].ToString());

                            //context.Categories.Add(cat);
                            categories.Add(cat);
                        }
                        //return HttpUtility.HtmlDecode(reader.ReadToEnd());
                    }
                }

                //context.SaveChanges();
            }
            catch (Exception exp)
            {

            }



            return categories;
        }

        public List<Data.Categorie> GetCategories()
        {
            List<Data.Categorie> categories = new List<Categorie>();
            try
            {
                WebRequest req = WebRequest.Create("https://api.paris.fr:3000/data/1.0/Equipements/get_categories/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
                        foreach (JObject categorie in jsonActivities["data"])
                        {
                            Categorie cat = new Categorie();
                            cat.Idcategorie = (int)categorie["idcategories"];
                            cat.Nom = WebUtility.HtmlDecode(categorie["name"].ToString());

                            categories.Add(cat);
                        }
                        //return HttpUtility.HtmlDecode(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception exp)
            {
                Categorie cat = new Categorie();

                cat.Nom = exp.Message + " " + exp.Source;
                cat.Idcategorie = 1;

                categories.Add(cat);
            }

            return categories;
        }

        public List<Data.Activity> GetAllActivities()
        {

            List<Data.Activity> activities = new List<Data.Activity>();

            try
            {
                WebRequest req = WebRequest.Create(@"https://api.paris.fr:3000/data/1.0/QueFaire/get_activities/?token=30539e0d4d810782e992a154e4dfa37bedb33652c6baf3fcbf7e6fd431482b23bbd8f892318ac3b58c45527e7aba721d&offset=0&limit=100");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
                        foreach (JObject activity in jsonActivities["data"])
                        {
                            Data.Activity act = new Data.Activity();
                            act.Adresse = activity["adresse"].ToString();
                            act.City = activity["city"].ToString();
                            act.Description = StripHTML(HttpUtility.HtmlDecode(activity["description"].ToString()));
                            act.ID = (int)activity["idactivites"];

                            activities.Add(act);
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception exp)
            {
                Data.Activity dumb = new Data.Activity();

                dumb.Adresse = exp.Message;
                dumb.City = exp.Source;
                dumb.Description = "dumb desc";
                dumb.ID = 1;

                activities.Add(dumb);
            }

            return activities;
        }

        public List<Tag> GetTags(string theme)
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

            context.Configuration.ProxyCreationEnabled = false;

            List<Tag> tags = context.Tags.Where(tg => tg.Theme.Title == theme).ToList<Tag>();

            return tags;
        }

        public void InjectTags(string theme)
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

            Tag tg = new Tag();
            tg.Title = "Concert";
            tg.Weight = 1;
            tg.ThemeID = 1;

            context.Tags.Add(tg);

            tg = new Tag();
            tg.Title = "Festival";
            tg.Weight = 1;
            tg.ThemeID = 1;

            context.Tags.Add(tg);

            tg = new Tag();
            tg.Title = "Jazz";
            tg.Weight = 1;
            tg.ThemeID = 1;

            context.Tags.Add(tg);

            tg = new Tag();
            tg.Title = "Classique";
            tg.Weight = 1;
            tg.ThemeID = 1;

            context.Tags.Add(tg);

            tg = new Tag();
            tg.Title = "Percussion";
            tg.Weight = 1;
            tg.ThemeID = 1;

            context.Tags.Add(tg);

            tg = new Tag();
            tg.Title = "Hip Hop";
            tg.Weight = 1;
            tg.ThemeID = 1;

            context.Tags.Add(tg);

            context.SaveChanges();
        }

        public List<int> PostInterests(string usernameid, Stream content)
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

            int userNameID = int.Parse(usernameid);
            // convert Stream Data to StreamReader
            StreamReader reader = new StreamReader(content);

            string result = reader.ReadToEnd();

            int tagID = int.Parse(result.Split('=')[1]);

            UserTag ut = context.UserTags.FirstOrDefault(userTag => userTag.TagID == tagID && userTag.UserID == userNameID);
            if (ut == null || ut.UserID == 0)
            {
                ut = new UserTag();
                ut.UserID = userNameID;
                ut.TagID = tagID;
                context.UserTags.Add(ut);
            }
            else
                context.UserTags.Remove(ut);

            context.SaveChanges();

            //return context.UserTags.Where(userTags => userTags.UserID == userNameID).Select(x => x.TagID).ToList<int>();

            return GetInterests(usernameid);

            //JObject jsonActivities = JObject.Parse(reader.ReadToEnd());
            //foreach (JObject activity in jsonActivities["data"])
            //{

            //}

            //return new List<int>();
        }

        public List<int> GetInterests(string usernameid)
        {
            TchillrDBContext context = new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

            List<int> results = new List<int>();
            int userNameID = int.Parse(usernameid);

            foreach (UserTag ut in context.UserTags.Where(userTag => userTag.UserID == userNameID))
            {
                results.Add(ut.TagID);
            }

            return results;
        }
    }
}