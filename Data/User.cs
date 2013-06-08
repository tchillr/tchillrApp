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
    public class User
    {
        [DataMember(Name = "identifier")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DataMember(Name = "nom")]
        public string Name { get; set; }

        [DataMember(Name = "prénom")]
        public string LastName { get; set; }

        [DataMember(Name = "interets")]
        public List<int> Interests { get; set; }
    }
}