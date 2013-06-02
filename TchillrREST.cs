using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace TchillrREST
{
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TchillrREST : ITchillrREST
    {
        public List<Data.Activity> GetAllActivities()
        {
            List<Data.Activity> activities = new List<Data.Activity>();

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
                        act.Description = activity["description"].ToString();
                        act.Idactivites = (int)activity["idactivites"];

                        activities.Add(act);
                    }
                }
            }
            else
            {
                Console.WriteLine(string.Format("Status Code: {0}, Status Description: {1}", resp.StatusCode, resp.StatusDescription));
            }
            Console.Read();

            return activities;
        }

    }
}