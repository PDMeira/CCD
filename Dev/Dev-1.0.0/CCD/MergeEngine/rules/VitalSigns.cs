
//    Developed by Masoud Hosseini

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcdInterfaces;
using System.Xml.Linq;

namespace MergeEngine.rules
{
    public class VitalSigns : DeDupRule, IDeDupRule
    {
        public string RuleName()
        {
            return "Vital Signs";
        }

        public int RuleVersion()
        {
            return 0;
        }

        public bool IsTest()
        {
            return false;
        }
        
        XElement dedupVitalSection; //Final deduplicated vital signs section
        public override void Merge()
        {
            foreach (XDocument ccd in CcdList) // extracts vital signs sections and compares to other ccds. Finally makes a deduplicated section
            {
                var tempVitalSection = GetSectionByCode(ccd, "8716-3");
                if (tempVitalSection != null)
                    VitalComparision(tempVitalSection);
            }

            var temp = GetSectionByCode(MasterCcd, "8716-3");
            if (dedupVitalSection != null)
                temp.ReplaceWith(dedupVitalSection);
        }

        void VitalComparision(XElement vitalSection)
        {
            if (dedupVitalSection == null) //Adds the first section into deduplicated vital section
                dedupVitalSection = vitalSection;
            else                            //else compare each entry of this section with the entries of deduplicated section
            {
                foreach (XElement e in vitalSection.Elements().Where(x => x.Name.LocalName == "entry"))
                    CompareEntryLevel(e);
            }
        }

        void CompareEntryLevel(XElement sectionElement) //compares in entry level and adds to deduplicated section if there is difference. If already there is the same entry, skip this.
        {
            bool duplicateElement = false;
            foreach (XElement e in dedupVitalSection.Elements().Where(x => x.Name.LocalName == "entry"))
            {
                bool time = e.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                    .Elements().FirstOrDefault(x => x.Name.LocalName == "effectiveTime")
                    .Attribute("value").Value
                    ==
                    sectionElement.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                    .Elements().FirstOrDefault(x => x.Name.LocalName == "effectiveTime")
                    .Attribute("value").Value;

                if (time) // in a same day. comapre the vital signs in that day
                {
                    bool blood = VitalComparison(e, sectionElement, "75367002", "Blood Pressure");
                    bool pulse = VitalComparison(e, sectionElement, "8499008", "Pulse");
                    bool respiration = VitalComparison(e, sectionElement, "86290005", "Respiration Rate");
                    bool height = VitalComparison(e, sectionElement, "50373000", "Height");
                    bool weight = VitalComparison(e, sectionElement, "27113001", "Weight");

                    if (blood && pulse && respiration && height && weight)
                    {
                        duplicateElement = true;
                        break;
                    }
                }

            }
            if (duplicateElement == false)
                dedupVitalSection.Add(sectionElement);
            else
                vitalDedupCount++; // counting overall number of deduplation 
        }

        bool VitalComparison(XElement dedupElement, XElement sectionElement, string code, string displayName)
        {
            var dedupObs = dedupElement.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "component")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "observation");
            var dedupVital = from dv in dedupObs.Descendants()
                             where dv.Name.LocalName == "code" && dv.Attribute("code").Value == code && dv.Attribute("displayName").Value == displayName
                             select dedupObs.Descendants().FirstOrDefault(x => x.Name.LocalName == "value").Value;


            var elementObs = sectionElement.Elements().FirstOrDefault(x => x.Name.LocalName == "organizer")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "component")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "observation");
            var elementVital = from v in elementObs.Descendants()
                               where v.Name.LocalName == "code" && v.Attribute("code").Value == "75367002"
                               select elementObs.Descendants().FirstOrDefault(x => x.Name.LocalName == "value").Value;

            return dedupVital == elementVital;
        }
    }
}
