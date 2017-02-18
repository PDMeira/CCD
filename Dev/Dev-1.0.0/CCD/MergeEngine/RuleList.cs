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
using System.Reflection;

namespace MergeEngine
{
    //public class RuleList
    //{
    //    //Return all Merge Rules
    //    public List<Type> MergeRules
    //    {
    //        get
    //        {
    //            return
    //                _ruleObjects.Where(
    //                    x => !(bool)x.InvokeMember("IsPrimary", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsPreFormat", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsPostFormat", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    ).ToList();
    //        }
    //    }

    //    //Return all Pre Format Rules
    //    public List<Type> PreFormatRules
    //    {
    //        get
    //        {
    //            return
    //                _ruleObjects.Where(
    //                    x => (bool)x.InvokeMember("IsPreFormat", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    ).ToList();
    //        }
    //    }

    //    //Return all Post Format Rules
    //    public List<Type> PostFormatRules
    //    {
    //        get
    //        {
    //            return
    //                _ruleObjects.Where(
    //                    x => (bool)x.InvokeMember("IsPostFormat", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsPrimary", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsPreFormat", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    )
    //                    .ToList();
    //        }
    //    }

    //    //Returns the primary rule
    //    public Type PrimaryRule
    //    {
    //        get
    //        {
    //            return
    //                _ruleObjects.First(
    //                    x => (bool)x.InvokeMember("IsPrimary", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    && !(bool)x.InvokeMember("IsTest", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x), null)
    //                    );
    //        }
    //    }

    //    private List<Type> _ruleObjects;

    //    public RuleList()
    //    {
    //        var ass = Assembly.Load("MergeEngine");

    //        _ruleObjects = (from s in ass.GetTypes()
    //                        where !s.IsInterface
    //                        where !s.IsAbstract
    //                        from i in s.GetInterfaces()
    //                        where i.Name == "IRule"
    //                        select s).ToList();
    //    }

    //}

    ///// <summary>
    ///// Different parts of CCD Header
    ///// </summary>
    public enum CcdHeaderParts
    {
        typeId,
        templateId,
        id,
        code,
        title,
        effectiveTime,
        confidentialityCode,
        languageCode,
        recordTarget,
        author,
        custodian,
        participant,
        informationRecipient,
        legalAuthenticator,
        documentationOf
    }
}
