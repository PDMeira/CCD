using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CcdInterfaces
{
    public interface IPreFormatRule : IRule, IDisposable
    {
        List<XDocument> GetCcdList();
        List<XElement> GetDiscarded();
        void BuildCcd(List<XDocument> ccdList);
    }
}
