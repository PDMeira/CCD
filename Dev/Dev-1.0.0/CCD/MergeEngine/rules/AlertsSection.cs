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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine.rules
{
    class AlertsSectionEntry
    {
        public XElement Element { get; set; }
        public string Code { get; set; }

    }

    public class AlertsSection : DeDupRule, IDeDupRule
    {
        private List<AlertsSectionEntry> _alertsSectionEntries;

        public AlertsSection()
        {
            _alertsSectionEntries = new List<AlertsSectionEntry>();
        }

        public string RuleName()
        {
            return "Alerts Section Consolidate";
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
            BuildAlertsSectionList();
            //RemoveNkdaIfOther();
            DedupAlerts();

            MergeToMaster(_alertsSectionEntries.Select(x => x.Element).ToList(), "48765-2");
        }

        private void DedupAlerts()
        {
            //Verify this will work
            for (var i = _alertsSectionEntries.Count - 1; i > -1; i--)
            {
                if (_alertsSectionEntries[i].Code == "")
                    continue;
                if (_alertsSectionEntries.Count(x => x.Code == _alertsSectionEntries[i].Code) > 1)
                {
                    _alertsSectionEntries.RemoveAt(i);
                    alertDedupCount++;// counting overall number of deduplation 
                }
            }
        }

        private void RemoveNkdaIfOther()
        {
            if (_alertsSectionEntries.Count(x => x.Code != "409137002") > 0)
                _alertsSectionEntries.RemoveAll(x => x.Code == "409137002");
        }

        private void BuildAlertsSectionList()
        {
            foreach (var i in CcdList)
            {
                var section = GetSectionByCode(i, "48765-2");

                var entries = section.Descendants().Where(x => x.Name.LocalName == "entry").ToList();

                foreach (var e in entries)
                {
                    try
                    {
                        _alertsSectionEntries.Add(new AlertsSectionEntry()
                                       {
                                           Code = e.Descendants().Elements().First(y =>
                                           {
                                               var xAttribute = y.Attribute("typeCode");
                                               return xAttribute != null &&
                                                      (y.Name.LocalName ==
                                                       "entryRelationship" &&
                                                       xAttribute.Value ==
                                                       "SUBJ");
                                           }).Descendants().First(
                                                                                           y =>
                                                                                           y.Name.LocalName ==
                                                                                           "participant").Descendants().
                                               First(y => y.Name.LocalName == "code").Attribute("code").Value,
                                           Element = e

                                       });
                    }
                    catch (Exception)
                    {

                        _alertsSectionEntries.Add(new AlertsSectionEntry()
                                                      {
                                                          Code = "",
                                                          Element = e
                                                      });
                    }

                }
            }
        }
    }
}
