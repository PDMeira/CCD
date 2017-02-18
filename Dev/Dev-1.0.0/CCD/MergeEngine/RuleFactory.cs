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
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine
{
    /// <summary>
    /// The rule factory is here to ensure proper construction of rules and to execute
    /// business logic that must be done prior to rule use.
    /// </summary>
    public static class RuleFactory
    {
        public static IPreFormatRule BuildPreFormatRule(Guid mergeId, string ruleName, int ruleVersion, List<XDocument> ccdList)
        {
            var retObj = (IPreFormatRule)Activator.CreateInstance(Assembly.Load("MergeEngine").GetTypes()
                .First(x => x.Name == ruleName 
                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
                    && (int) x.InvokeMember("RuleVersion", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null) == ruleVersion));

            retObj.BuildCcd(ccdList);
            retObj.BuildAudit(retObj, mergeId);

            return retObj;
        }

        public static IPrimaryRule BuildPrimaryRule(Guid mergeId, string ruleName, int ruleVersion, List<XDocument> ccdList)
        {
            var retObj = (IPrimaryRule)Activator.CreateInstance(Assembly.Load("MergeEngine").GetTypes()
                .First(x => x.Name == ruleName
                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
                    && (int)x.InvokeMember("RuleVersion", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null) == ruleVersion));

            retObj.BuildCcd(ccdList);
            retObj.BuildAudit(retObj, mergeId);

            return retObj;
        }

        public static IDeDupRule BuildDeDupRule(Guid mergeId, string ruleName, int ruleVersion, List<XDocument> ccdList, XDocument masterCcd)
        {
            var retObj = (IDeDupRule)Activator.CreateInstance(Assembly.Load("MergeEngine").GetTypes()
                .First(x => x.Name == ruleName
                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
                    && (int)x.InvokeMember("RuleVersion", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null) == ruleVersion));

            retObj.BuildCcd(ccdList, masterCcd);
            retObj.BuildAudit(retObj, mergeId);

            return retObj;
        }

        public static IPostFormatRule BuildPostFormatRule(Guid mergeId, string ruleName, int ruleVersion, XDocument masterCcd)
        {
            var retObj = (IPostFormatRule)Activator.CreateInstance(Assembly.Load("MergeEngine").GetTypes()
                .First(x => x.Name == ruleName
                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
                    && (int)x.InvokeMember("RuleVersion", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null) == ruleVersion));

            retObj.BuildCcd(masterCcd);
            retObj.BuildAudit(retObj, mergeId);

            return retObj;
        }

        //public static IRule BuildRule(Guid mergeId, Type rule, List<XDocument> ccdList, XDocument masterCcd)
        //{
            
        //    //This will sweep the assembly for a rul that matches the type passed and construct it
        //    var retObj = (IRule) Activator.CreateInstance(Assembly.Load("MergeEngine").GetTypes()
        //        .First(x => x == rule && !(bool)x.InvokeMember("IsTest",BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)));
            
        //    //This will execute logic to ensure proper construction and ensure auditing is not bypassed
        //    //By putting audit in the factory pattern we prevent future developers from developing
        //    //a rule that does not write an audit record

        //    //retObj.BuildCcd(ccdList, masterCcd);
        //    retObj.BuildAudit(retObj, mergeId);

        //    retObj.SetMergeMessage(retObj.RuleName());

        //    //returns constructed object
        //    return retObj;
        //}

        ///// <summary>
        ///// BuildByRuleSet reads in the rules set sent with the Merge Request and constructs a list of merge rules based on the ruleset
        ///// </summary>
        ///// <param name="mergeId">Uniqe GUID for the Merge Job</param>
        ///// <param name="ruleSet">Ruleset constructed for the Job</param>
        ///// <param name="ccdList">CCD's to Dedup</param>
        ///// <returns></returns>
        //public static List<IRule> BuildByRuleSet(Guid mergeId, RuleSet ruleSet, List<XDocument> ccdList, bool xx)
        //{
        //    //Reads in the ruleSet and loads the activated objects into a list
        //    var retObjs = ruleSet.GetCompleteRuleSet().Select(i => (IRule) Activator.CreateInstance(
        //        Assembly.Load("MergeEngine").GetTypes()
        //        .First(x => x.Name == i.RuleName 
        //            && !(bool) x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null) 
        //            && (int) x.InvokeMember("RuleVersion", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null) == i.RuleVersion))
        //            ).ToList();

        //    //retObjs.ForEach(x => x.BuildCcd(ccdList, null));
        //    retObjs.ForEach(x => x.BuildAudit(x, mergeId));

        //    retObjs.ForEach(x => x.SetMergeMessage(x.RuleName()));

        //    return retObjs;
        //}

    }
}
