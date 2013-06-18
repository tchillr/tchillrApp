using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Configuration;
using Newtonsoft.Json;

namespace TchillrREST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TchillrWCFService" in code, svc and config file together.
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TchillrWCFService : ITchillrWCFService
    {
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

        public List<int> GetInterests(string usernameid)
        {

            List<int> results = new List<int>();
            int userNameID = int.Parse(usernameid);

            foreach (TchillrREST.DataModel.User ut in TchillrREST.Utilities.TchillrContext.Users.Where(user => user.identifier == userNameID))
            {
                results.Add(ut.Tag.identifier);
            }

            return results;
        }

        public string GetActivitiesForDays(string nbDays)
        {
            TchillrREST.Utilities.TchillrContext.ContextOptions.ProxyCreationEnabled = false;
            DateTime till = DateTime.Now.AddDays(double.Parse(nbDays));
            var activitiesForDays = from acti in TchillrREST.Utilities.TchillrContext.Activities
                                    from occ in TchillrREST.Utilities.TchillrContext.Occurences
                                    where acti.identifier == occ.ActivityID && occ.jour > DateTime.Now && occ.jour < till
                                        select acti;

            return JsonConvert.SerializeObject(activitiesForDays.ToList());
        }
    }
}
