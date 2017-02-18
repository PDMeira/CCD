// CCD Consolidation and De-duplication Engine
// Copyright (C) 2013  CreateIT Healthcare Solutions, Inc.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see {http://www.gnu.org/licenses/}.

// Contact Info:
//     Jonathan Meade
//     CreateIT Healthcare Solutions, Inc.
//     516 SW 15th Street
//     Richmond, IN 47374
//     jmeade@createit-inc.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MergeEngine;
using System.Linq;
using System.IO;
using CcdInterfaces;

namespace UI
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MergeCCDs(new List<string>());
            }
            
        }

        protected void MergeCCD_Click(object sender, EventArgs e)
        {
            var excludeRules = new List<string>();
            excludeRules.AddRange(form2.Controls.OfType<CheckBox>().Where(c => !c.Checked).Select(x => x.ID));

            MergeCCDs(excludeRules);

        }
       
        protected void MergeCCDs(List<string> excludeRules)
        {
            var ccdList = new List<XDocument>();

            //var ruleList = new RuleList();
            var ruleSet = DefaultRuleset.GetDefaultRuleset();

            ccdList.Add(XDocument.Load(Server.MapPath("~/HHIC_CCD_1.xml")));
            ccdList.Add(XDocument.Load(Server.MapPath("~/HHIC_CCD_2.xml")));
            ccdList.Add(XDocument.Load(Server.MapPath("~/HHIC_CCD_3.xml")));

            TxtCcd1.InnerText = ccdList[0].ToString();
            TxtCcd2.InnerText = ccdList[1].ToString();
            TxtCcd3.InnerText = ccdList[2].ToString();

            XDocument MasterCcd = XDocument.Parse("<empty />");

            var mergeId = Guid.NewGuid();

            foreach (var i in ruleSet.PreFormatRules.Where(x => !excludeRules.Contains(x.RuleName)))
            {
                using (var xRule = RuleFactory.BuildPreFormatRule(mergeId, i.RuleName, 0, ccdList))
                {
                    xRule.Merge();
                }
            }

            using (var xRule = RuleFactory.BuildPrimaryRule(mergeId, ruleSet.PrimaryRule.RuleName, 0, ccdList))
            {
                xRule.Merge();
                MasterCcd = xRule.GetMasterCcd();
            }

            foreach (var i in ruleSet.DeDuplicationRules.Where(x => !excludeRules.Contains(x.RuleName)))
            {
                using (var xRule = RuleFactory.BuildDeDupRule(mergeId, i.RuleName, 0, ccdList, MasterCcd))
                {
                    xRule.Merge();
                    MasterCcd = xRule.GetMasterCcd();
                }
            }

            foreach (var i in ruleSet.PostFormatRules.Where(x => !excludeRules.Contains(x.RuleName)))
            {
                using (var xRule = RuleFactory.BuildPostFormatRule(mergeId, i.RuleName, 0, MasterCcd))
                {
                    xRule.Merge();
                    MasterCcd = xRule.GetMasterCcd();
                }
            }

            TxtMasterCcd.InnerText = MasterCcd.ToString();
            //var mem = new MemoryStream();
            
            //// Code used after creating package above...  (notice the ref mem).
            //HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=System.DateTime.Now" + ".docx");
            //mem.Position = 0;
            //mem.CopyTo(HttpContext.Current.Response.OutputStream);
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();

        }

        

        
    }

}