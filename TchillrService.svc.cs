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
            List<Data.Activity> activities = new List<Data.Activity>();

            try
            {
                WebRequest req = WebRequest.Create("http://" + HttpContext.Current.Request.Url.Authority + "/staticActivy.txt");

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
                            act.Occurences = new List<Occurence>();
                            act.Nom = WebUtility.HtmlDecode(activity["nom"].ToString());
                            act.Adresse = WebUtility.HtmlDecode(activity["adresse"].ToString());
                            act.City = WebUtility.HtmlDecode(activity["city"].ToString());
                            act.Description = StripHTML(WebUtility.HtmlDecode(activity["description"].ToString()));
                            act.Idactivites = (int)activity["idactivites"];
                            act.Zipcode = activity["zipcode"].ToString();
                            act.ShortDescription = string.Empty;

                            float temp = 0;
                            if (float.TryParse(activity["lat"].ToString(), out temp))
                                act.Lat = temp;
                            temp = 0;
                            if (float.TryParse(activity["lon"].ToString(), out temp))
                                act.Lon = temp;

                            foreach (JObject occ in activity["occurences"])
                            {
#warning convertir en start date end date
                                Occurence occurence = new Occurence();
                                occurence.Day = occ["jour"].ToString();
                                occurence.StartTime = occ["hour_start"].ToString();
                                occurence.EndTime = occ["hour_end"].ToString();
                                act.Occurences.Add(occurence);
                            }

                            act.Keywords = act.GetKeywords();

                            activities.Add(act);
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
                dumb.Idactivites = 1;

                activities.Add(dumb);
            }

            return activities;
        }

        public List<Data.Categorie> GetStaticCategories()
        {
            List<Data.Categorie> categories = new List<Categorie>();
            try
            {
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

                            categories.Add(cat);
                        }
                        //return HttpUtility.HtmlDecode(reader.ReadToEnd());
                    }
                }
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
                            act.Idactivites = (int)activity["idactivites"];

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
                dumb.Idactivites = 1;

                activities.Add(dumb);
            }

            return activities;
        }

    }
}
