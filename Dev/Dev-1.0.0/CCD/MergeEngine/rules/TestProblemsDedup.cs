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
    /// This is a class used to hold information fro the elements list.  Allows us
    /// to pull out information to do comparisons without haveing to reach into the XML
    /// </summary>
    class TestProblemsDedup_Entry
    {
        public int CcdListIndex { get; set; }
        public int ComponentListIndex { get; set; }
        public XElement Element { get; set; }
        public string SnomedCode
        {
            
            
            get { return snomedCode ?? ICD9ToSnomed.First(x => x.Key == OtherCode).Value; }
            set { snomedCode = value; }
        }
        
        public string OtherCode { get; set; }
        public string CodeDef { get; set; }

        private string snomedCode;

        
        public Dictionary<string, string> ICD9ToSnomed;

        public TestProblemsDedup_Entry()
        {
            ICD9ToSnomed = new Dictionary<string, string>();
            ICD9ToSnomed.Add("276.8", "43339004");
            ICD9ToSnomed.Add("729.1", "268108002");
            ICD9ToSnomed.Add("786.05", "158379001");
            ICD9ToSnomed.Add("786.50", "29857009");
            ICD9ToSnomed.Add("786.52", "22253000");
        }

    }

    /// <summary>
    /// Rule to merge and deduplicate the problems section
    /// </summary>
    public class TestProblemsDedup : DeDupRule, IDeDupRule
    {
        //public override XDocument MergeCCDs(List<XDocument> ccdList, XDocument masterCcd)
        //{
        //    var problemList = new List<TestProblemsDedup_Entry>();
           
        //    //Geting the focus section of the ccd for all ccds in the list provided
        //    for (var i = 0; i < ccdList.Count; i++)
        //    {
        //        var comp = GetComponentByCode(ccdList[i], "11450-4");

        //        var entries = (from e in comp.Descendants()
        //                       where e.Name.LocalName == "entry"
        //                       select e).ToList();

        //        //Check for non-Snomed entries
        //        var problemListCount = problemList.Count;


        //        //Get Entries with SNOMED
        //        for (var ie = 0; ie < entries.Count; ie++)
        //        {
                    
        //            var snomeCode = entries[ie].Descendants().First(x =>
        //                                                                {
        //                                                                    XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance"; 
        //                                                                    var xAttribute = x.Attribute(ns + "type");
        //                                                                    return xAttribute != null && (x.Name.LocalName == "value" && xAttribute.Value == "CD");
        //                                                                }).Attribute("code");
                    
        //            if (snomeCode != null)
        //                problemList.Add(new TestProblemsDedup_Entry()
        //                                    {
        //                                        CcdListIndex = i,
        //                                        ComponentListIndex = ie,
        //                                        Element = entries[ie],
        //                                        SnomedCode = snomeCode.Value
        //                                    });
        //        }

        //        //Get Entries with no SNOMED
        //        //This could be removed if we validate the CCD is c32 format
        //        if (problemList.Count - entries.Count != problemListCount)
        //        {
        //            for (var ie = 0; ie < entries.Count; ie++)
        //            {
        //                var valueTag = entries[ie].Descendants().First(x =>
        //                            {
        //                                XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
        //                                var xAttribute = x.Attribute(ns + "type");
        //                                return xAttribute != null && (x.Name.LocalName == "value" && xAttribute.Value == "CD");
        //                            });

        //                if (valueTag != null)
        //                {
        //                    var otherCode = valueTag.Descendants().First(x => x.Name.LocalName == "translation").Attribute("code");
        //                    var otherCodeDef = valueTag.Descendants().First(x => x.Name.LocalName == "translation").Attribute("codeSystem");
                            
        //                    if (otherCode !=null && otherCodeDef != null)
        //                        problemList.Add(new TestProblemsDedup_Entry()
        //                            {
        //                                CcdListIndex = i,
        //                                ComponentListIndex = ie,
        //                                Element = entries[ie],
        //                                OtherCode = otherCode.Value,
        //                                CodeDef = otherCodeDef.Value
        //                            });
                            
        //                }
        //            }
                
        //        }

                

        //    }

        //    //Consolidate to Master

        //    //---------------NEED REAL DEDUP HERE------------
        //    for (int index = 0; index < problemList.Count; index++)
        //    {
        //        var i = problemList[index];
        //        if (problemList.Count(x => x.SnomedCode == i.SnomedCode) > 1)
        //        {
        //            //Put real logic for conslidation
        //            problemList.Remove(i);
        //        }
        //    }

        //    //------------------------------------------------

        //    //Merges and returns ccd
        //    return MergeToMaster(masterCcd, problemList.Select(x => x.Element).ToList(), "11450-4");

        //    //masterCcd.Descendants().Last(x => x.Name.LocalName == "section"
        //    //    && x.Elements().Count(y =>
        //    //        { 
        //    //            var yAttribute = y.Attribute("code");
        //    //            return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == "11450-4");
        //    //        }) > 0).Descendants().Elements().Where(x => x.Name.LocalName == "entry").Remove();

        //    //foreach (var i in problemList)
        //    //{
        //    //    masterCcd.Descendants().Last(x => x.Name.LocalName == "section"
        //    //    && x.Elements().Count(y =>
        //    //        {
        //    //            var yAttribute = y.Attribute("code");
        //    //            return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == "11450-4");
        //    //        }) > 0).Add(i.Element);

        //    //}
            
        //}

        
        public string RuleName()
        {
            return "Test Problems Deduplication Rule";
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
            var problemList = new List<TestProblemsDedup_Entry>();

            //Geting the focus section of the ccd for all ccds in the list provided
            for (var i = 0; i < CcdList.Count; i++)
            {
                var comp = GetComponentByCode(CcdList[i], "11450-4");

                var entries = (from e in comp.Descendants()
                               where e.Name.LocalName == "entry"
                               select e).ToList();

                //Check for non-Snomed entries
                var problemListCount = problemList.Count;


                //Get Entries with SNOMED
                for (var ie = 0; ie < entries.Count; ie++)
                {

                    var snomeCode = entries[ie].Descendants().First(x =>
                    {
                        var xAttribute = x.Attribute(ns + "type");
                        return xAttribute != null && (x.Name.LocalName == "value" && xAttribute.Value == "CD");
                    }).Attribute("code");

                    if (snomeCode != null)
                        problemList.Add(new TestProblemsDedup_Entry()
                        {
                            CcdListIndex = i,
                            ComponentListIndex = ie,
                            Element = entries[ie],
                            SnomedCode = snomeCode.Value
                        });
                }

                //Get Entries with no SNOMED
                //This could be removed if we validate the CCD is c32 format
                if (problemList.Count - entries.Count != problemListCount)
                {
                    for (var ie = 0; ie < entries.Count; ie++)
                    {
                        var valueTag = entries[ie].Descendants().First(x =>
                        {
                            var xAttribute = x.Attribute(ns + "type");
                            return xAttribute != null && (x.Name.LocalName == "value" && xAttribute.Value == "CD");
                        });

                        try
                        {
                            if (valueTag != null)
                            {
                                var otherCode = valueTag.Descendants().First(x => x.Name.LocalName == "translation").Attribute("code");
                                var otherCodeDef = valueTag.Descendants().First(x => x.Name.LocalName == "translation").Attribute("codeSystem");

                                if (otherCode != null && otherCodeDef != null)
                                    problemList.Add(new TestProblemsDedup_Entry()
                                    {
                                        CcdListIndex = i,
                                        ComponentListIndex = ie,
                                        Element = entries[ie],
                                        OtherCode = otherCode.Value,
                                        CodeDef = otherCodeDef.Value
                                    });

                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                        
                    }

                }



            }

            //Consolidate to Master

            //---------------NEED DEDUP HERE------------
            for (int index = 0; index < problemList.Count; index++)
            {
                var i = problemList[index];
                if (problemList.Count(x => x.SnomedCode == i.SnomedCode) > 1)
                {
                    //Put logic for conslidation
                    problemList.Remove(i);
                    problemsDedupCount++; // counting overall number of deduplation 
                }
            }

            //------------------------------------------------

            //Merges and returns ccd
            MergeToMaster(problemList.Select(x => x.Element).ToList(), "11450-4");
        }
    }
}
