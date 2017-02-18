
//    Developed by Masoud Hosseini

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine.rules
{
    public class Immunization : DeDupRule, IDeDupRule
    {
        public string RuleName()
        {
            return "Immunization";
        }

        public int RuleVersion()
        {
            return 0;
        }

        public bool IsTest()
        {
            return false;
        }

        XElement dedupImmunSection; //Final deduplicated Immunization section
        public override void Merge()
        {
            foreach (XDocument ccd in CcdList) // extracts Immunization sections and compares to other ccds. Finally makes a deduplicated section
            {
                var tempImmunSection = GetSectionByCode(ccd, "11369-6");
                if (tempImmunSection != null)
                    ImmunComparision(tempImmunSection);
            }

            var temp = GetSectionByCode(MasterCcd, "11369-6");
            temp.ReplaceWith(dedupImmunSection);

        }

        void ImmunComparision(XElement immunSection)
        {
            if (dedupImmunSection == null) //Adds the first section into deduplicated immunization section
                dedupImmunSection = immunSection;
            else                            //else compare each entry of this section with the entries of deduplicated section
            {
                foreach (XElement e in immunSection.Elements().Where(x => x.Name.LocalName == "entry"))
                    CompareEntryLevel(e);
            }
        }

        void CompareEntryLevel(XElement sectionElement) //compares in entry level and adds to deduplicated section if there is difference. If already there is the same entry, skip this.
        {
            bool duplicateElement = false;
            foreach (XElement e in dedupImmunSection.Elements().Where(x => x.Name.LocalName == "entry"))
            {

                bool time = e.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "effectiveTime")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "center")
                     .Attribute("value").Value
                     ==
                     sectionElement.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "effectiveTime")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "center")
                     .Attribute("value").Value;

                bool cptCode = e.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "consumable")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "manufacturedProduct")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "manufacturedMaterial")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "code")
                     .Attribute("code").Value
                     ==
                     sectionElement.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "consumable")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "manufacturedProduct")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "manufacturedMaterial")
                     .Elements().FirstOrDefault(x => x.Name.LocalName == "code")
                     .Attribute("code").Value;

                if (time & cptCode)
                {
                    duplicateElement = true;
                    break;
                }

            }
            if (duplicateElement == false)
                dedupImmunSection.Add(sectionElement);
        }

    }
}
