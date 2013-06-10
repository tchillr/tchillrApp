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
    public class WordCloud
    {
        [DataMember(Name = "identifier")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [ForeignKey("Tag")]
        public int TagID { get; set; }

        public virtual Tag Tag { get; set; }
    }
}