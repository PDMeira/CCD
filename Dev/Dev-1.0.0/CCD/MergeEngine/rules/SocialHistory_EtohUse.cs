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
using MergeEngine;

namespace MergeEngine.rules
{
    public enum EtohUsePriority
    {
        UnKnown = 0,
        None = 10,
        Social = 20,
        Heavy = 30
    }

    /// <summary>
    /// This class is using a very small synonym engine.  In a production a full medical term synonym engine could be
    /// Used to accomplish what is done with this simple library. 
    /// </summary>
    class EtohUseEntry
    {
        public EtohUsePriority Priority
        {
            get
            {
                var findPriority = EtohUsePriority.UnKnown;
                var testStrings = Entry.Descendants().Elements().First(x => x.Name.LocalName == "value").Value.Split(' ');

                foreach (var s in testStrings)
                {
                    if (findPriority < _etohKeyWords.FirstOrDefault(x => x.Key == s).Value)
                        findPriority = _etohKeyWords.FirstOrDefault(x => x.Key == s).Value;
                }

                return findPriority;
            }
        }
        public XElement Entry { get; set; }

        private Dictionary<string, EtohUsePriority> _etohKeyWords;
        
        public EtohUseEntry()
        {
            //See class summary
            _etohKeyWords = new Dictionary<string, EtohUsePriority>()
                                                               {
                                                                   {"No", EtohUsePriority.None},
                                                                   {"None", EtohUsePriority.None},
                                                                   {"NA", EtohUsePriority.None},
                                                                   {"Denies", EtohUsePriority.None},
                                                                   {"Social", EtohUsePriority.Social},
                                                                   {"Light", EtohUsePriority.Social},
                                                                   {"Month", EtohUsePriority.Social},
                                                                   {"Heavy", EtohUsePriority.Heavy},
                                                                   {"Alchoholic", EtohUsePriority.Heavy},
                                                                   {"Daily", EtohUsePriority.Heavy}
                                                               };
        }
        

    }

    public class SocialHistory_EtohUse : DeDupRule, IDeDupRule
    {
        

        public string RuleName()
        {
            return "ETOH Use Consolidation";
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
            var etohEntry = new EtohUseEntry();

            foreach (var i in CcdList)
            {
                try
                {
                    var section = GetSectionByCode(i, "29762-2");
                    var entries = from e in section.Descendants()
                                  where e.Name.LocalName == "entry"
                                  where e.Descendants().Elements().Count(x =>
                                  {
                                      var xAttribute = x.Attribute("code");
                                      return xAttribute != null && xAttribute.Value == "160573003";
                                  }) > 0
                                  select e;
                    var sEntry = entries.FirstOrDefault();

                    var testEtoh = new EtohUseEntry() {Entry = sEntry};

                    if (sEntry != null)
                    {
                        if (etohEntry.Entry == null)
                            etohEntry.Entry = sEntry;
                        else
                        {
                            if (etohEntry.Priority < testEtoh.Priority)
                            {
                                etohEntry = testEtoh;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //Not great
                }


            }

            MergeToMasterSingleEntry(etohEntry.Entry, "29762-2", "160573003");
        }
    }
}
