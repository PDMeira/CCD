using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AwsInterface;

namespace CcdDeDupAsService
{
    public partial class completed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateList();
            //var completedMerge = S3Interface.GetMergeSet(Guid.Parse(mergeId), "Whatever");
        }
        private void UpdateList()
        {

            var listOfCompletedMerges = S3Interface.GetCompletedMerges();
            var sb = new StringBuilder();

            sb.Append("<ul list-style-type:none;padding:0px;margin:0px;>");
            const string strRef = "<a href =\"#\" onclick=\"loadXml('{0}')\">CCD_{1}_{2}</a>";
            foreach (var s in from s in listOfCompletedMerges let lb = new LinkButton() select s)
            {
                sb.AppendFormat("<li style=\"background-image:url(images/sqorange.gif);background-repeat:no-repeat;background-position:40px 3px; padding-left:56px;list-style-type: none;\">{0}</li>", string.Format(strRef, s.Value, DateTime.Parse(s.Key).ToShortDateString(), DateTime.Parse(s.Key).ToShortTimeString()));
            }
            sb.Append("</ul>");
            divContent.InnerHtml = sb.ToString();
        }

        public void LoadCCD()
        {
            var i = 1;
        }
    }


}