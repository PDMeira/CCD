using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Audit;

namespace MergeEngine
{
	public abstract class PreFormatRule : Rule, IDisposable
	{
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
