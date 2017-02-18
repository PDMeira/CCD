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
using CcdInterfaces;

namespace MergeEngine
{
    public class PrimaryMergeRuleWithValidation : PrimaryRule, IPrimaryRule
    {
        public string RuleName()
        {
            return "Primary By Latest Valid CCD";
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
            string[] dateTimeFormats = new string[] { "yyyyMMddHHmmss.fffzzz", "yyyyMMddHHmmsszzz" };
            //Need to add validation
            MasterCcd = (from r in CcdList

                         where
                             DateTimeOffset.ParseExact(
                                 r.Descendants().First(x => x.Name.LocalName == "effectiveTime").Attribute("value").
                                     Value.ToString(), dateTimeFormats, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None) ==
                             CcdList.Max(
                                 x =>
                                 DateTimeOffset.ParseExact(
                                     x.Descendants().First(i => i.Name.LocalName == "effectiveTime").Attribute("value").
                                         Value.ToString(), dateTimeFormats, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None))
                         select r).First();
        }
    }
}
