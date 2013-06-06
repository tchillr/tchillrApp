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
                            act.Adresse = activity["adresse"].ToString();
                            act.City = activity["city"].ToString();
                            act.Description = StripHTML(HttpUtility.HtmlDecode(activity["description"].ToString()));
                            act.Idactivites = (int)activity["idactivites"];

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

        public string GetStaticCategories()
        {
            try
            {
                WebRequest req = WebRequest.Create("http://" + HttpContext.Current.Request.Url.Authority + "/categories.json");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        return HttpUtility.HtmlDecode(reader.ReadToEnd());
                        
                    }
                }
            }
            catch (Exception exp)
            {
                return exp.Message + exp.Source;
            }

            return string.Empty;
        }

    }
}
