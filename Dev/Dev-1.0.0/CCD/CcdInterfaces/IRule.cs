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

namespace CcdInterfaces
{
    /// <summary>
    /// All rules should implement the IRule interface
    /// The IRule interface will be used when we do reflection to pull in new rules with just the dll
    /// </summary>
    public interface IRule
    {
        //Used by Rule Reflection
        string RuleName(); //<--should return a unique string name from the rule
        int RuleVersion();//<-- If we ever want to change a rule this would be handy, not yet used
        //RuleType GetRuleType(); //<-- Replaces IsPrimary, IsPreformat, IsPostFormat
        //bool IsPrimary();//<-- If this is set to true, it will be the first rule run in the merge phase
        //bool IsPreFormat();//<-- If true this will run in the formating phase
        // IsPostFormat();//<--Formating rule used after the ccd's are merged and deduped
        bool IsTest();//<--Allows for test environment

        void BuildAudit(object ruleObject, Guid mergeId);
        void SetMergeMessage(string ruleName);
        void Merge();
        string GetAuditRecordBson();


        //Functional

    }
}
