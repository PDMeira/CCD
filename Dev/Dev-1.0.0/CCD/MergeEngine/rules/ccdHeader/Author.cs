using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine.rules
{
    class Author : DeDupRule, IDeDupRule
    {

        public string RuleName()
        {
            return "Author Consolidation";
        }

        public int RuleVersion()
        {
            return 0;
        }

        public bool IsPrimary()
        {
            return false;
        }

        public bool IsPreFormat()
        {
            return false;
        }

        public bool IsPostFormat()
        {
            return false;
        }

        public bool IsTest()
        {
            return false;
        }

        public override void Merge()
        {
            var authors = GetHeaderPartsByName(CcdList, CcdHeaderParts.author);
            List<string> tempNpid = new List<string>(); //Nationl Provider Identifier
            List<XElement> tempAuthors = new List<XElement>();

            foreach (XElement author in authors)
            {
                var npid = author.Elements().FirstOrDefault(x => x.Name.LocalName == "assignedAuthor")
                    .Elements().FirstOrDefault(x => x.Name.LocalName == "id")
                    .Attribute("root").Value;

                if (!tempNpid.Contains(npid.ToString()))
                {
                    tempNpid.Add(npid.ToString());
                    tempAuthors.Add(author);
                }
                else
                    authorDedupCount++;// counting overall number of deduplation 
            }
            if (tempAuthors.Count > 0)
            {
                MasterCcd.Root.Elements().Where(x => x.Name.LocalName == "author").Remove(); //remove author from Master CCD

                // Add author (Machine). This author is the organization by which the ccd's are consolidated
                // Probably this should be added dynamically not hard coded

                //XNamespace xmlns = @"urn:hl7-org:v3";
                //MasterCcd.Root.Elements().FirstOrDefault(x => x.Name.LocalName == "recordTarget").AddAfterSelf(new XElement(xmlns + "author")); 
                
                foreach (XElement author in tempAuthors)
                    MasterCcd.Root.Elements().FirstOrDefault(x => x.Name.LocalName == "recordTarget").AddAfterSelf(author);
            }
        }
    }
}
