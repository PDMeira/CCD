using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine.rules
{
    public class Confidentiality : DeDupRule, IDeDupRule
    {

        public string RuleName()
        {
            return "Confidentiality Consolidate";
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

        struct ConfidentialityStruct
        {
            Int16 priority;

            public Int16 Priority
            {
                get { return priority; }
                set { priority = value; }
            }
            string code;
            string dispalyName;

            public string DispalyName
            {
                get { return dispalyName; }
                set { dispalyName = value; }
            }

            public string Code
            {
                get { return code; }
                set { code = value; }
            }
        }

        public override void Merge()
        {
            ConfidentialityStruct lowPriority = new ConfidentialityStruct() { Priority = 1, Code = "U", DispalyName = "unrestricted" }; // lowest priority


            List<ConfidentialityStruct> confList = new List<ConfidentialityStruct>();
            confList.Add(new ConfidentialityStruct() { Priority = 1, Code = "U", DispalyName = "unrestricted" });
            //Definition: Privacy metadata indicating that the information is not classified as sensitive. Examples: Includes publicly available information, e.g., business name, phone, email or physical address. Usage Note: This metadata indicates that the receiver has no obligation to consider additional policies when making access control decisions. Note that in some jurisdictions, personally identifiable information must be protected as confidential, so it would not be appropriate to assign a confidentiality code of "unrestricted" to that information even if it is publicly available.
            confList.Add(new ConfidentialityStruct() { Priority = 2, Code = "L", DispalyName = "low" });
            //Definition: Privacy metadata indicating that the information has been de-identified, and there are mitigating circumstances that prevent re-identification, which minimize risk of harm from unauthorized disclosure. The information requires protection to maintain low sensitivity. Examples: Includes anonymized, pseudonymized, or non-personally identifiable information such as HIPAA limited data sets. Map: No clear map to ISO 13606-4 Sensitivity Level (1) Care Management: RECORD_COMPONENTs that might need to be accessed by a wide range of administrative staff to manage the subject of care's access to health services. Usage Note: This metadata indicates the receiver may have an obligation to comply with a data use agreement.
            confList.Add(new ConfidentialityStruct() { Priority = 3, Code = "M", DispalyName = "moderate" });
            //Definition: Privacy metadata indicating moderately sensitive information, which presents moderate risk of harm if disclosed without authorization. Examples: Includes allergies of non-sensitive nature used inform food service; health information a patient authorizes to be used for marketing, released to a bank for a health credit card or savings account; or information in personal health record systems that are not governed under health privacy laws. Map: Partial Map to ISO 13606-4 Sensitivity Level (2) Clinical Management: Less sensitive RECORD_COMPONENTs that might need to be accessed by a wider range of personnel not all of whom are actively caring for the patient (e.g. radiology staff). Usage Note: This metadata indicates that the receiver may be obligated to comply with the receiver's terms of use or privacy policies.
            confList.Add(new ConfidentialityStruct() { Priority = 4, Code = "N", DispalyName = "normal" });
            //Definition: Privacy metadata indicating that the information is typical, non-stigmatizing health information, which presents typical risk of harm if disclosed without authorization. Examples: In the US, this includes what HIPAA identifies as the minimum necessary protected health information (PHI) given a covered purpose of use (treatment, payment, or operations). Includes typical, non-stigmatizing health information disclosed in an application for health, workers compensation, disability, or life insurance. Map: Partial Map to ISO 13606-4 Sensitivity Level (3) Clinical Care: Default for normal clinical care access (i.e. most clinical staff directly caring for the patient should be able to access nearly all of the EHR). Maps to normal confidentiality for treatment information but not to ancillary care, payment and operations. Usage Note: This metadata indicates that the receiver may be obligated to comply with applicable jurisdictional privacy law or disclosure authorization.
            confList.Add(new ConfidentialityStruct() { Priority = 5, Code = "R", DispalyName = "restricted" });
            //Definition: Privacy metadata indicating highly sensitive, potentially stigmatizing information, which presents a high risk to the information subject if disclosed without authorization. May be preempted by jurisdictional law, e.g., for public health reporting or emergency treatment.> Examples: In the US, this includes what HIPAA identifies as the minimum necessary protected health information (PHI) given a covered purpose of use (treatment, payment, or operations). Includes typical, non-stigmatizing health information disclosed in an application for health, workers compensation, disability, or life insurance. Map: Partial Map to ISO 13606-4 Sensitivity Level (3) Clinical Care: Default for normal clinical care access (i.e. most clinical staff directly caring for the patient should be able to access nearly all of the EHR). Maps to normal confidentiality for treatment information but not to ancillary care, payment and operations. Usage Note: This metadata indicates that the receiver may be obligated to comply with applicable, prevailing (default) jurisdictional privacy law or disclosure authorization.
            confList.Add(new ConfidentialityStruct() { Priority = 6, Code = "V", DispalyName = "very restricted" });
            //Definition: Privacy metadata indicating that the information is extremely sensitive and likely stigmatizing health information that presents a very high risk if disclosed without authorization. This information must be kept in the highest confidence. Examples: Includes information about a victim of abuse, patient requested information sensitivity, and taboo subjects relating to health status that must be discussed with the patient by an attending provider before sharing with the patient. May also include information held under â€œlegal lockâ€? or attorney-client privilege Map: This metadata indicates that the receiver may not disclose this information except as directed by the information custodian, who may be the information subject. Usage Note: This metadata indicates that the receiver may not disclose this information except as directed by the information custodian, who may be the information subject.


            //  string[] confidentialityCodes = { "L", "M", "N", "R", "U", "V", }; 

            var r = GetHeaderPartsByName(CcdList, CcdHeaderParts.confidentialityCode);
            // Getting the most confidential code from ccds
            r.ForEach(x =>
                {
                    foreach (ConfidentialityStruct cs in confList)
                    {
                        if (x.Attribute("code").Value == cs.Code && lowPriority.Priority < cs.Priority)
                        {
                            lowPriority = cs;
                            continue;
                        }
                    }
                });

            // creating a new Confidentialit node
            XNamespace xmlns = @"urn:hl7-org:v3";
            var tempNode = new XElement(xmlns + "confidentialityCode");

            tempNode.Add(new XAttribute("code", lowPriority.Code));
            tempNode.Add(new XAttribute("codeSystemName", "Confidentiality"));
            tempNode.Add(new XAttribute("displayName", lowPriority.DispalyName));
            tempNode.Add(new XAttribute("codeSystem", "2.16.840.1.113883.5.25"));

            ///replacing with the previous one
            var c = MasterCcd.Root.Elements().FirstOrDefault(x => x.Name.LocalName == CcdHeaderParts.confidentialityCode.ToString());
            c.ReplaceWith(tempNode);
            confidentialityDedupCount++;// counting overall number of deduplation 

        }

    }
}
