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
using Audit;

namespace MergeEngine
{
    /// <summary>
    /// All rules should inherit from this abstract class.  
    /// The Abstract method MergCCDs is forced to be used on all rules
    /// </summary>
    public abstract class Rule
    {
        //Public Members
        //public List<XDocument> CcdList;
        //public XDocument MasterCcd;  Not pre
        //public List<XElement> Discarded;
        public AuditRecord AuditRecord;

        public readonly XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";

        public XComment MergeMessage = new XComment("Merged Information By CCD DeDuplication Engine");

        public void SetMergeMessage(string ruleName)
        {
            MergeMessage = new XComment("Merged Information By CCD DeDuplication Engine **Rule: " + ruleName + "**");
        }

        //public XDocument GetMasterCcd()  Not Pre
        //{
        //    //AuditRecord.Complete(CcdList, MasterCcd, Discarded);
        //    //AuditWritter.WriteAuditRecord(AuditRecord);
        //    //AuditRecord = null;
        //    return MasterCcd;
        //}

        public abstract void Merge();

        public void BuildAudit(object ruleObject, Guid mergeId)
        {
            AuditRecord = new AuditRecord(ruleObject, mergeId);
        }

        protected XElement GetComponentByCode(XDocument ccd, string code)
        {
            var compList = (from e in ccd.Descendants()
                            where e.Name.LocalName == "component"
                            where e.Descendants().Elements().Count(x =>
                            {
                                var xAttribute = x.Attribute("code");
                                return xAttribute != null && (x.Name.LocalName == "code" && xAttribute.Value == code);
                            }) > 0
                            select e).ToList();

            return compList[1];
        }

        /// <summary>
        /// Same as the component by code but goes down one level to return the section
        /// </summary>
        /// <param name="ccd"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected XElement GetSectionByCode(XDocument ccd, string code)
        {
            try
            {
                var compList = (from e in ccd.Descendants().Descendants()
                                where e.Name.LocalName == "section"
                                where e.Elements().Count(x =>
                                {
                                    var xAttribute = x.Attribute("code");
                                    return xAttribute != null && (x.Name.LocalName == "code" && xAttribute.Value == code);
                                }) > 0
                                select e).ToList();

                return compList[0];
            }
            catch (Exception)
            {
                return null;
            }

        }
        //protected XElement GetSectionByTemplateId(XDocument ccd, string root)
        //{
        //    try
        //    {
        //        var compList = (from e in ccd.Descendants().Descendants()
        //                        where e.Name.LocalName == "section"
        //                        where e.Elements().Count(x =>
        //                        {
        //                            var xAttribute = x.Attribute("templateId");
        //                            return xAttribute != null && (x.Name.LocalName == "root" && xAttribute.Value == root);
        //                        }) > 0
        //                        select e).ToList();

        //        return compList[0];
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }

        //}

        public List<XElement> GetHeaderPartsByName(List<XDocument> ccdList, CcdHeaderParts headerPartName) // Returns the first level nondes in CCD header based on filter 
        {
            List<XElement> r = new List<XElement>();

            foreach (var ccd in ccdList)
            {
                List<XElement> temp = (from e in ccd.Root.Elements()
                                       where e.Name.LocalName == headerPartName.ToString()
                                       select e).ToList();
                temp.ForEach(x => r.Add(x));
            }

            return r;
        }

        public string GetAuditRecordBson()
        {
            return AuditRecord.ToJson();

        }

    }


}
