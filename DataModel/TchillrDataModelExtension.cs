using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TchillrREST.DataModel
{
    public partial class Activity
    {
        [DataMemberAttribute()]
        public List<ContextualTag> tags { get; set; }

        [DataMemberAttribute()]
        public int score { get; set; }

        [DataMemberAttribute(Name = "Occurences")]
        public List<DataModel.Occurence>  OccurencesToSend{ get; set; }
    }

    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public class ContextualTag
    {
        [DataMemberAttribute()]
        public int identifier { get; set; }

        [DataMemberAttribute()]
        public string title { get; set; }
    }
}