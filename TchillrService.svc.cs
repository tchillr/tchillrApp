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

        public string GetStaticAllActivities()
        {
            string result = string.Empty;

            try
            {
                WebRequest req = WebRequest.Create("http://" + HttpContext.Current.Request.Url.Authority + "/staticActivy.txt");
                string s = "zal";
                req.Method = "GET";

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                        StripHTML(HttpUtility.HtmlDecode(result));
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
