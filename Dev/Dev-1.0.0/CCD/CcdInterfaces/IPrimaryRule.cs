using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CcdInterfaces
{
    public interface IPrimaryRule : IRule, IDisposable
    {
        void BuildCcd(List<XDocument> ccdList);
        XDocument GetMasterCcd();
        List<XDocument> GetCcdList();
    }
}
