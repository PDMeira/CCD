using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Audit;

namespace MergeEngine
{
    public abstract class PostFormatRule : Rule
    {
        public XDocument MasterCcd;

        public void BuildCcd(XDocument masterCcd)
        {
            MasterCcd = masterCcd;
        }

        public XDocument GetMasterCcd()
        {
            return MasterCcd;
        }

        //public void BuildAudit(Guid mergeId, string mergeRule, int ruleVersion)
        //{
        //    AuditRecord = new AuditRecord(mergeId, mergeRule, ruleVersion, CcdList, MasterCcd);
        //}


        public void Dispose()
        {
            if (AuditRecord != null)
                AuditRecord.Complete(this);
            AuditWritter.WriteAuditRecord(AuditRecord);
        }
    }
}
