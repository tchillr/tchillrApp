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
        [DataMember]
        public int Idactivites { get; set; }

        [DataMember]
        public string Nom { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Lieu { get; set; }

        [DataMember]
        public string Adresse { get; set; }

        [DataMember]
        public string Zipcode { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public float Lat { get; set; }

        [DataMember]
        public float Lon { get; set; }

        [DataMember]
        public List<DateTime> Occurences { get; set; }
    }
}