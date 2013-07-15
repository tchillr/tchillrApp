using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TchillrREST
{
    public partial class LogReader : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            divLog.InnerHtml = Server.MapPath("~/TchillrLog/Log.txt");
            //try
            //{
            //    string line;
            //    // Read the file and display it line by line.
            //    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\TchillrLog\Log.txt");
            //    while ((line = file.ReadLine()) != null)
            //    {
            //        divLog.InnerHtml += line + "<br>";
            //    }
            //    file.Close();
            //}
            //catch (Exception exp)
            //{
            //    divLog.InnerHtml += exp.Message;
            //}
        }
    }
}