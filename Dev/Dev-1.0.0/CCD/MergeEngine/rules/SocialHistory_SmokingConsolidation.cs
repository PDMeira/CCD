// CCD Consolidation and De-duplication Engine
// Copyright (C) 2013  CreateIT Healthcare Solutions, Inc.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see {http://www.gnu.org/licenses/}.

// Contact Info:
//     Jonathan Meade
//     CreateIT Healthcare Solutions, Inc.
//     516 SW 15th Street
//     Richmond, IN 47374
//     jmeade@createit-inc.com

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine.rules
{
    class SocialHistory_SmokingConsolidation : DeDupRule, IDeDupRule
    {
        public string RuleName()
        {
            return "Social History Smoking History";
        }

        public int RuleVersion()
        {
            return 0;
        }

        public bool IsTest()
        {
            return false;
        }

        public override void Merge()
        {
            XElement smokeEntry = null;

            foreach (var i in CcdList)
            {
                try
                {
                    var section = GetSectionByCode(i, "29762-2");
                    if (section != null)
                    {
                        var entries = from e in section.Descendants()
                                      where e.Name.LocalName == "entry"
                                      where e.Descendants().Elements().Count(x =>
                                      {
                                          var xAttribute = x.Attribute("code");
                                          return xAttribute != null && xAttribute.Value == "230056004";
                                      }) > 0
                                      select e;
                        var sEntry = entries.FirstOrDefault();

                        if (sEntry != null)
                        {
                            if (smokeEntry == null)
                                smokeEntry = sEntry;
                            else
                            {
                                if (sEntry.Descendants().Elements().Count(x => x.Name.LocalName == "effectiveTime") > 0)
                                    smokeEntry = sEntry;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //Not great
                }


            }
            if (smokeEntry != null)
                MergeToMasterSingleEntry(smokeEntry, "29762-2", "230056004");

        }
    }
}
