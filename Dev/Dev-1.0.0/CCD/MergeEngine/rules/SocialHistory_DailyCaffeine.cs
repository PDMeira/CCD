
//    Developed by Masoud Hosseini

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcdInterfaces;
using System.Xml.Linq;

namespace MergeEngine.rules
{
    public class SocialHistory_DailyCaffeine : DeDupRule, IDeDupRule
    {
        public string RuleName()
        {
            return "Social History Daily Caffeine";
        }

        public int RuleVersion()
        {
            return 0;
        }

        public bool IsTest()
        {
            return false;
        }

        public override void Merge()
        {
            List<XElement> dcEntry = new List<XElement>();

            foreach (var i in CcdList)
            {
                try
                {
                    var section = GetSectionByCode(i, "29762-2");
                    if (section != null)
                    {
                        var entries = from e in section.Descendants()
                                      where e.Name.LocalName == "entry"
                                      where e.Descendants().Elements().Count(x =>
                                      {
                                          var xAttCode = x.Attribute("code");
                                          var xAttDipsName = x.Attribute("displayName");

                                          return xAttCode != null && xAttDipsName != null && xAttCode.Value == "160476009" && xAttDipsName.Value == "Daily Caffeine";
                                      }) > 0
                                      select e;
                        dcEntry.Add(entries.FirstOrDefault());

                    }
                }
                catch (Exception)
                {

                }
            }

            //removes elements from the section

            MasterCcd.Descendants().Elements().Last(x => x.Name.LocalName == "section" && x.Elements().Count(y =>
            {
                var yAttribute = y.Attribute("code");
                return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == "29762-2");
            }) > 0).Descendants().Where(x => x.Name.LocalName == "entry" && x.Descendants().Elements().Count(y =>
            {
                var attributeCode = y.Attribute("code");
                var attributeDispName = y.Attribute("displayName");
                return attributeCode != null && attributeDispName != null && (y.Name.LocalName == "code" && attributeCode.Value == "160476009" && attributeDispName.Value == "Daily Caffeine");
            }) > 0).Remove();


            //Adds the new element
            foreach (XElement e in dcEntry)
                MasterCcd.Descendants().Last(x => x.Name.LocalName == "section"
                    && x.Elements().Count(y =>
                    {
                        var yAttribute = y.Attribute("code");
                        return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == "29762-2");
                    }) > 0).Add(e);
        }
    }
}
