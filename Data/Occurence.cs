using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TchillrREST.Data
{
    [DataContract]
    public class Occurence
    {
        [Key, Column(Order = 1)]
        [DataMember(Name = "jour")]
        public string Day { get; set; }

        [Key, Column(Order = 2)]
        [DataMember(Name = "hour_start")]
        public string StartTime { get; set; }

        [Key, Column(Order = 3)]
        [DataMember(Name = "hour_end")]
        public string EndTime { get; set; }

        [Key, Column(Order = 0)]
        [ForeignKey("Activity")]
        public int ActivityID { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
