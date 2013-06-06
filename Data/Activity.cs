using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TchillrREST.Data
{
    [DataContract]
    public class Activity
    {
        [DataMember(Name = "identifier")]
        public int Idactivites { get; set; }

        [DataMember(Name = "name")]
        public string Nom { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "shortDescription")]
        public string ShortDescription { get; set; }

        [DataMember(Name = "place")]
        public string Lieu { get; set; }

        [DataMember(Name = "adress")]
        public string Adresse { get; set; }

        [DataMember(Name = "zipcode")]
        public string Zipcode { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "latitude")]
        public float Lat { get; set; }

        [DataMember(Name = "longitude")]
        public float Lon { get; set; }

        [DataMember(Name = "occurences")]
        public List<Occurence> Occurences { get; set; }
    }
}