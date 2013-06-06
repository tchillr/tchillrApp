using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TchillrREST.Data
{
    [DataContract]
    public class Categorie
    {
        [DataMember(Name = "identifier")]
        public int Idcategorie { get; set; }

        [DataMember(Name = "name")]
        public string Nom { get; set; }
    }
}