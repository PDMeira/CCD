using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Audit;

namespace MergeEngine
{
    public abstract class PrimaryRule : Rule, IDisposable
    {
        public XDocument MasterCcd;
        public List<XDocument> CcdList;

        public List<XDocument> GetCcdList()
        {
            return CcdList;
        }
        public XDocument GetMasterCcd()
        {
            //AuditRecord.Complete(CcdList, MasterCcd, Discarded);
            //AuditWritter.WriteAuditRecord(AuditRecord);
            //AuditRecord = null;
            return MasterCcd;
        }

        public void BuildCcd(List<XDocument> ccdList)
        {
            CcdList = ccdList;
        }

        
        public void Dispose()
        {
            if (AuditRecord != null)
                AuditRecord.Complete(this);
            AuditWritter.WriteAuditRecord(AuditRecord);
        }
    }
}
