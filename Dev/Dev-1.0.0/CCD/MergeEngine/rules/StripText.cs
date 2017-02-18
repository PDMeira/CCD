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

namespace MergeEngine
{

    /// <summary>
    /// This is the formating rule that removes all infor from the text tabs
    /// </summary>
    public class StripText: PreFormatRule, IPreFormatRule
    {
        public string RuleName()
        {
            return "Strip HTML Text from CCDs";
        }

        public int RuleVersion()
        {
            return 0;
        }

        
        public bool IsTest()
        {
            return false;
        }

        //This is assuming html tables are used.  A more percise method should
        //be written to remove all data from the text tags
        private void FormatCCDs()
        {
            foreach (var i in CcdList)
            {
                i.Descendants().Elements().Where(x => x.Name.LocalName == "table").Remove();
            }

        }

        public override void Merge()
        {
            FormatCCDs();
        }
    }
}
