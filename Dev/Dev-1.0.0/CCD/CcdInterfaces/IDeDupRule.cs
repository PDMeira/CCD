using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CcdInterfaces
{
    public interface IDeDupRule : IRule, IDisposable
    {
        void BuildCcd(List<XDocument> ccdList, XDocument masterCcd);
        XDocument GetMasterCcd();
        void SetMergeMessage(string ruleName);
        List<XDocument> GetCcdList();
        List<XElement> GetDiscarded();
    }
}
