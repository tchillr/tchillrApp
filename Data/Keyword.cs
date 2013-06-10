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
    public class Keyword
    {
        [DataMember(Name = "identifier")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "hits")]
        public int Hits { get; set; }

        [ForeignKey("Activity")]
        public int ActivityID { get; set; }

        public virtual Activity Activity { get; set; }

    }
}