using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TchillrREST.Data
{
    [DataContract]
    public class Occurence
    {
        [DataMember(Name = "jour")]
        public string Day { get; set; }

        [DataMember(Name = "hour_start")]
        public string StartTime { get; set; }

        [DataMember(Name = "hour_end")]
        public string EndTime { get; set; }

    }
}
