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

        [DataMemberAttribute()]
        public string color { get; set; }

        [DataMemberAttribute()]
        public Attendance attendance { get; set; }

        [DataMemberAttribute(Name = "Occurences")]
        public List<DataModel.Occurence> OccurencesToSend { get; set; }

        public void cleanActivity()
        {

            if (this.name.Contains("&"))
            {
                this.name = System.Web.HttpUtility.HtmlDecode(this.name);
            }

            if (this.description.Contains("&"))
            {
                this.description = System.Web.HttpUtility.HtmlDecode(this.description);
            }

            if (this.shortDescription.Contains("&"))
            {
                this.shortDescription = System.Web.HttpUtility.HtmlDecode(this.shortDescription);
            }

            if (this.place.Contains("&"))
            {
                this.place = System.Web.HttpUtility.HtmlDecode(this.place);
            }

            if (this.adress.Contains("&"))
            {
                this.adress = System.Web.HttpUtility.HtmlDecode(this.adress);
            }

            if (this.city.Contains("&"))
            {
                this.city = System.Web.HttpUtility.HtmlDecode(this.city);
            }

            if (this.accessType.Contains("&"))
            {
                this.accessType = System.Web.HttpUtility.HtmlDecode(this.accessType);
            }

            if (this.price.Contains("&"))
            {
                this.price = System.Web.HttpUtility.HtmlDecode(this.price);
            }

            if (this.metro.Contains("&"))
            {
                this.metro = System.Web.HttpUtility.HtmlDecode(this.metro);
            }

            if (this.velib.Contains("&"))
            {
                this.velib = System.Web.HttpUtility.HtmlDecode(this.velib);
            }

            if (this.bus.Contains("&"))
            {
                this.bus = System.Web.HttpUtility.HtmlDecode(this.bus);
            }

            if (this.organisateur != null && this.organisateur.Contains("&"))
            {
                this.organisateur = System.Web.HttpUtility.HtmlDecode(this.organisateur);
            }

            if (this.hasFee.Contains("&"))
            {
                this.hasFee = System.Web.HttpUtility.HtmlDecode(this.hasFee);
            }

        }
    }

    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public class ContextualTag
    {
        [DataMemberAttribute()]
        public int identifier { get; set; }

        [DataMemberAttribute()]
        public string title { get; set; }

        [DataMemberAttribute()]
        public int themeID { get; set; }
    }

    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public struct Attendance
    {
        [DataMemberAttribute()]
        public int yes { get; set; }

        [DataMemberAttribute()]
        public int no { get; set; }

        [DataMemberAttribute()]
        public int maybe { get; set; }
    }
}