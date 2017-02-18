using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using CcdDeDupAsService.HttpUtils;
using RestService;

namespace CcdDeDupAsService
{
    public partial class upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string guidKey = Request.QueryString["key"];
            Guid g;
            if (Guid.TryParse(guidKey, out g))
            {
                var r = new CcdDeDup();
                try
                {
                    var jsonRule = Request.Form[0];
                    if (jsonRule.Length > 0)
                    {
                        r.MergeWithRuleSet("1", "2", g.ToString(), jsonRule);
                        r.FireMergeEngine();
                    }
                    else
                    {
                        r.MergeWithDefaultRules("1", "2", g.ToString());
                        r.FireMergeEngine();
                    }
                    return;
                }
                catch
                {

                }

                int loop1;
                HttpFileCollection Files;
                Files = Request.Files; // Load File collection into HttpFileCollection variable.
                var arr1 = Files.AllKeys; // This will get names of all files into a string array. 
                for (loop1 = 0; loop1 < arr1.Length; loop1++)
                {
                    var f = Server.HtmlEncode(arr1[loop1]);
                    var s = Files[loop1].ContentLength;
                    var c = Files[loop1].ContentType;
                    var i = Files[loop1].InputStream;
                    //var xmlInput = XDocument.Load(new StreamReader(i));
                    if (Files[loop1].ContentType == "text/xml")
                    {
                        r.AddCcd("1", "2", g.ToString(), i);
                    }
                    
                }
            }

        }
    }
}