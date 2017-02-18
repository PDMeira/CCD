using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CcdInterfaces
{
    public interface IPostFormatRule : IRule, IDisposable
    {
        XDocument GetMasterCcd();

        void BuildCcd(XDocument masterCcd);
    }
}
