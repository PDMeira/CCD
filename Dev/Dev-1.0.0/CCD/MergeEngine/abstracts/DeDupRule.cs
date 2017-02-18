using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Audit;

namespace MergeEngine
{
    public abstract class DeDupRule : Rule, IDisposable
    {
        public static int vitalDedupCount = 0;
        public static int problemsDedupCount = 0;
        public static int alertDedupCount = 0;
        public static int medicationDedupCount = 0;
        public static int confidentialityDedupCount = 0;
        public static int authorDedupCount = 0;

        public XDocument MasterCcd;
        public List<XDocument> CcdList;
        public List<XElement> Discarded;

        public List<XDocument> GetCcdList()
        {
            return CcdList;
        }

        public List<XElement> GetDiscarded()
        {
            return Discarded;
        }

        public XDocument GetMasterCcd()
        {
            //AuditRecord.Complete(CcdList, MasterCcd, Discarded);
            //AuditWritter.WriteAuditRecord(AuditRecord);
            //AuditRecord = null;
            return MasterCcd;
        }
        
        protected void MergeToMaster(List<XElement> elements, string code)
        {
            //Ensure Master has section
            //TODO need to figure out next primary master

            if (MasterCcd.Descendants().Elements().Count(x =>
            {
                var xAttribute = x.Attribute("code");
                return xAttribute != null && (x.Name.LocalName == "code" && xAttribute.Value == code);
            }) == 0)
            {
                var insertComponent = GetComponentByCode(CcdList[0], code);
                MasterCcd.Descendants().First(x => x.Name.LocalName == "structuredBody").Add(insertComponent);
            }


            //removes elements from the section

            MasterCcd.Descendants().Elements().Last(x => x.Name.LocalName == "section" && x.Elements().Count(y =>
            {
                var yAttribute = y.Attribute("code");
                return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == code);
            }) > 0).Elements().Where(x => x.Name.LocalName == "entry").Remove();

            //Adds each element in the list to the master ccd
            foreach (var i in elements)
            {
                i.AddFirst(MergeMessage);

                MasterCcd.Descendants().Last(x => x.Name.LocalName == "section"
                && x.Elements().Count(y =>
                {
                    var yAttribute = y.Attribute("code");
                    return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == code);
                }) > 0).Add(i);
            }


        }

        protected void MergeToMasterSingleEntry(XElement element, string sectionCode, string entryCode)
        {
            //Add Merge Comment
            element.AddFirst(MergeMessage);

            //Ensure Master has section
            //TODO need to figure out next primary master

            if (MasterCcd.Descendants().Elements().Count(x =>
            {
                var xAttribute = x.Attribute("code");
                return xAttribute != null && (x.Name.LocalName == "code" && xAttribute.Value == sectionCode);
            }) == 0)
            {
                var insertComponent = GetComponentByCode(CcdList[0], sectionCode);
                MasterCcd.Descendants().First(x => x.Name.LocalName == "structuredBody").Add(insertComponent);
            }

            //removes elements from the section

            MasterCcd.Descendants().Elements().Last(x => x.Name.LocalName == "section" && x.Elements().Count(y =>
            {
                var yAttribute = y.Attribute("code");
                return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == sectionCode);
            }) > 0).Descendants().Where(x => x.Name.LocalName == "entry" && x.Descendants().Elements().Count(y =>
            {
                var attribute = y.Attribute("code");
                return attribute != null && (y.Name.LocalName == "code" && attribute.Value == entryCode);
            }) > 0).Remove();

            //Adds the new element
            MasterCcd.Descendants().Last(x => x.Name.LocalName == "section"
                && x.Elements().Count(y =>
                {
                    var yAttribute = y.Attribute("code");
                    return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == sectionCode);
                }) > 0).Add(element);


        }

        public void BuildAudit(Guid mergeId, string mergeRule, int ruleVersion)
        {
            AuditRecord = new AuditRecord(mergeId, mergeRule, ruleVersion, CcdList, MasterCcd);
        }

        public void BuildCcd(List<XDocument> ccdList, XDocument masterCcd)
        {
            CcdList = ccdList;
            MasterCcd = masterCcd;
            Discarded = new List<XElement>();
        }

        public void Dispose()
        {
            if (AuditRecord != null)
                AuditRecord.Complete(this);
            AuditWritter.WriteAuditRecord(AuditRecord);
        }
    }
}
