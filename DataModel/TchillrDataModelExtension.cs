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
        public List<DataModel.Tag> tags { get; set; }

        [DataMemberAttribute()]
        public int score { get; set; }
    }
}