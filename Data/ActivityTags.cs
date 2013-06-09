using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TchillrREST.Data
{
    [DataContract]
    public class ActivityTags
    {
        [DataMember(Name = "activityIdentifier")]
        [Key, Column(Order = 0)]
        public int ActivityID { get; set; }

        [DataMember(Name = "tagIdentifier")]
        [Key, Column(Order = 1)]
        public int TagID { get; set; }
    }
}