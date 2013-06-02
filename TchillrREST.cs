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
                            act.Description = activity["description"].ToString();
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

        public string GetAllVLibStations()
        {
            string result = string.Empty;
            try
            {
                WebRequest req = WebRequest.Create(@"https://api.jcdecaux.com/vls/v1/stations?apiKey=0413b1cd2774059bf6ffc962f300a61307dcce45");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }
                }

            }
            catch (Exception exp)
            {
                result = exp.Message;
            }
            return result;
        }


        public string GetStaticAllActivities()
        {
            string result = string.Empty;

            try
            {
                WebRequest req = WebRequest.Create("http://"+HttpContext.Current.Request.Url.Authority+"/staticActivy.txt");

                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        result = reader.ReadToEnd();

                    }
                }
            }
            catch (Exception exp)
            {
               
            }

            return result;
        }
    }
}