
//    Developed by Masoud Hosseini

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcdInterfaces;
using System.Xml.Linq;

namespace MergeEngine.rules
{
    public class FamilyHistory : DeDupRule, IDeDupRule
    {
        public string RuleName()
        {
            return "Family History";
        }

        public int RuleVersion()
        {
            return 0;
        }

        public bool IsTest()
        {
            return false;
        }

        XElement dedupFamilySection; //Final deduplicated family history section
        public override void Merge()
        {
            foreach (XDocument ccd in CcdList) // extracts family sections and compares to other ccds. Finally makes a deduplicated section
            {
                var tempFamily = GetSectionByCode(ccd, "10157-6");
                if (tempFamily != null)
                    FamilyHistoryComparision(tempFamily);
            }


            var temp = GetSectionByCode(MasterCcd, "10157-6");
            temp.ReplaceWith(dedupFamilySection);

        }

        void FamilyHistoryComparision(XElement familySection)
        {
            if (dedupFamilySection == null) //Adds the first section into deduplicated family section
                dedupFamilySection = familySection;
            else                            //else compare each entry of this section with the entries of deduplicated section
            {
                foreach (XElement e in familySection.Elements().Where(x => x.Name.LocalName == "entry"))
                    CompareEntryLevel(e);
            }
        }

        void CompareEntryLevel(XElement sectionElement) //compares in entry level and adds to deduplicated section if there is difference. If already there is the same entry, skip this.
        {
            bool duplicateElement = false;
            foreach (XElement e in dedupFamilySection.Elements().Where(x => x.Name.LocalName == "entry"))
            {

                bool member = e.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "subject")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "relatedSubject")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "code")
                     .Attribute("displayName").Value
                     ==
                     sectionElement.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "subject")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "relatedSubject")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "code")
                     .Attribute("displayName").Value;

                bool problem = e.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "component")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "observation")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "value")
                     .Attribute("displayName").Value
                     ==
                     sectionElement.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "component")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "observation")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "value")
                     .Attribute("displayName").Value;

                if (member & problem)
                {
                    duplicateElement = true;
                    break;
                }

            }
            if (duplicateElement == false)
                dedupFamilySection.Add(sectionElement);
        }

    }
}
