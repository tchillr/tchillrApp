﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TchillrREST.Data
{
    [DataContract]
    public class Theme
    {
        [DataMember(Name = "identifier")]
        [Key]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
    }
}