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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Linq;
using MergeEngine;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AwsInterface;


namespace TestApp
{
    class testclass : IDisposable, ICollection
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var readyToMerge = S3Interface.GetReadyToMerge();

            foreach (var m in readyToMerge)
            {
                
                var ccdList = S3Interface.GetMergeSetCcds(Guid.Parse(m)).Select(x => XDocument.Parse(x)).ToList();
                var sRuleSet = S3Interface.GetMergeSetRules(Guid.Parse(m));

                RuleSet ruleSet;

                if (!sRuleSet.Contains("{\"UseDefault\": \"true\"}"))
                    ruleSet = new RuleSet(sRuleSet);
                else
                {
                    ruleSet = new RuleSet();
                    ruleSet.AddRule("StripText", 0, RuleType.PreFormat);
                    ruleSet.AddRule("PrimaryMergeRuleWithValidation", 0, RuleType.Primary);
                    ruleSet.AddRule("MedicationRule1", 0, RuleType.DeDuplication);
                    ruleSet.AddRule("AlertsSection", 0, RuleType.DeDuplication);
                    ruleSet.AddRule("SocialHistory_EtohUser", 0, RuleType.DeDuplication);
                    ruleSet.AddRule("SocialHistory_SmokingConsolidation", 0, RuleType.DeDuplication);
                    ruleSet.AddRule("TestProblemsDedup", 0, RuleType.DeDuplication);
                    ruleSet.AddRule("AddFormatedText", 0, RuleType.PostFormat);
                }


                S3Interface.AddMergedZip(Guid.Parse(m), RunMerge.runMergeWithRuleSet(ccdList, ruleSet, Guid.Parse(m),""));
            }
            //var mergeList = S3Interface.GetReadyToMerge();

            //foreach (var m in mergeList)
            //{
            //    var xx = S3Interface.GetMergeSetCcds(Guid.Parse(m));

            //    foreach (var x in xx)
            //    {
            //        var test = XDocument.Parse(x);
            //    }
            //}

            

            //var xx = new testclass();

            //var xxx = xx.GetType().GetInterfaces().Where(x => x.GetInterfaces().Select(y => y.Name).Contains("IEnumerable"));

            //var testRuleSet = new RuleSet();

            //var postRules = new List<RuleDefinition>
            //                    {
            //                        new RuleDefinition
            //                            {RuleName = "PostTest1", RuleType = RuleType.PostFormat, RuleVersion = 0},
            //                        new RuleDefinition
            //                            {RuleName = "PostTest2", RuleType = RuleType.PostFormat, RuleVersion = 0}
            //                    };

            //var mergeRules = new List<RuleDefinition>
            //                    {
            //                        new RuleDefinition
            //                            {RuleName = "MergeTest1", RuleType = RuleType.DeDuplication, RuleVersion = 0},
            //                        new RuleDefinition
            //                            {RuleName = "MergeTest2", RuleType = RuleType.DeDuplication, RuleVersion = 0}
            //                    };

            //var preRules = new List<RuleDefinition>
            //                    {
            //                        new RuleDefinition
            //                            {RuleName = "PreTest1", RuleType = RuleType.PreFormat, RuleVersion = 0},
            //                        new RuleDefinition
            //                            {RuleName = "PreTest2", RuleType = RuleType.PreFormat, RuleVersion = 0}
            //                    };

            //testRuleSet.PostFormatRules = postRules;
            //testRuleSet.PreFormatRules = preRules;
            //testRuleSet.DeDuplicationRules = mergeRules;
            //testRuleSet.PrimaryRule = new RuleDefinition { RuleName = "PrimaryRule", RuleType = RuleType.Primary, RuleVersion = 0 };

            //var ruleSet = new RuleSet();
            //ruleSet.AddRule("StripText", 0, RuleType.PreFormat);
            //ruleSet.AddRule("PrimaryMergeRuleWithValidation", 0, RuleType.Primary);
            //ruleSet.AddRule("MedicationRule1", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("AlertsSection", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("SocialHistory_EtohUser", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("SocialHistory_SmokingConsolidation", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("TestProblemsDedup", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("AddFormatedText", 0, RuleType.PostFormat);

            //var testJson = JsonConvert.SerializeObject(ruleSet);

            //StringBuilder sb = new StringBuilder(517);
            //sb.AppendLine(@"{");
            //sb.AppendLine(@"""PreFormatRules"":");
            //sb.AppendLine(@"[");
            //sb.AppendLine(@"{""RuleName"":""PreTest1"",""RuleVersion"":0,""GetRuleType"":10},");
            //sb.AppendLine(@"{""RuleName"":""PreTest2"",""RuleVersion"":0,""GetRuleType"":10}");
            //sb.AppendLine(@"],");
            //sb.AppendLine(@"""PrimaryRule"":{""RuleName"":""PrimaryRule"",""RuleVersion"":0,""GetRuleType"":20},""DeDuplicationRules"":");
            //sb.AppendLine(@"[");
            //sb.AppendLine(@"{""RuleName"":""MergeTest1"",""RuleVersion"":0,""GetRuleType"":30},");
            //sb.AppendLine(@"{""RuleName"":""MergeTest2"",""RuleVersion"":0,""GetRuleType"":30}");
            //sb.AppendLine(@"],");
            //sb.AppendLine(@"""PostFormatRules"":");
            //sb.AppendLine(@"[");
            //sb.AppendLine(@"{""RuleName"":""PostTest1"",""RuleVersion"":0,""GetRuleType"":40},");
            //sb.AppendLine(@"{""RuleName"":""PostTest2"",""RuleVersion"":0,""GetRuleType"":40}");
            //sb.AppendLine(@"]");
            //sb.AppendLine(@"}");

            //var xx = sb.ToString();

            //var ds = new RuleSet(sb.ToString());


            ////Create a ccd list of XDocuments
            //var ccdList = new List<XDocument>();

            //ccdList.Add(XDocument.Load(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_1.xml"));
            //ccdList.Add(XDocument.Load(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_2.xml"));
            //ccdList.Add(XDocument.Load(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_3.xml"));


            //XDocument xMaster = XDocument.Parse("<empty />");


            //var ruleList = new RuleList();

            //foreach (var i in ruleList.PreFormatRules)
            //{
            //    using (var xRule = RuleFactory.BuildRule(Guid.NewGuid(), i, ccdList, xMaster))
            //    {
            //        xRule.Merge();
            //        xMaster = xRule.GetMasterCcd();
            //    }
            //}

            //using (var xRule = RuleFactory.BuildRule(Guid.NewGuid(), ruleList.PrimaryRule, ccdList, xMaster))
            //{
            //    xRule.Merge();
            //    xMaster = xRule.GetMasterCcd();
            //}


            //foreach (var i in ruleList.MergeRules)
            //{
            //    using (var xRule = RuleFactory.BuildRule(Guid.NewGuid(), i, ccdList, xMaster))
            //    {
            //        var startTime = DateTime.Now;
            //        xRule.Merge();
            //        xMaster = xRule.GetMasterCcd();
            //        Console.WriteLine("RunTime: " + (DateTime.Now - startTime).TotalSeconds.ToString());
            //    }
            //}

            //foreach (var i in ruleList.PostFormatRules)
            //{
            //    using (var xRule = RuleFactory.BuildRule(Guid.NewGuid(), i, ccdList, xMaster))
            //    {
            //        xRule.Merge();
            //        xMaster = xRule.GetMasterCcd();
            //    }
            //}

            //Console.Write("Done");


            ////Run the format rule
            //var preRule = new StripText();
            //ccdList = preRule.FormatCCDs(ccdList);

            ////Run the primary rule
            //var priRule = new PrimaryMergeRule();
            //var masterCCD = priRule.MergeCCDs(ccdList, null);

            ////Run test consolidation rules
            //var testRule = new TestProblemsDedup();

            //masterCCD = testRule.MergeCCDs(ccdList, masterCCD);

            //var medRule = new MedicationRule1();
            //masterCCD = medRule.MergeCCDs(ccdList, masterCCD);

            //foreach (var ccd in ccdList)
            //{
            //    var compList = (from e in ccd.Descendants()
            //               where e.Name.LocalName == "component"
            //               where e.Descendants().Elements().Count(x =>
            //                                                          {
            //                                                              var xAttribute = x.Attribute("code");
            //                                                              return xAttribute != null && (x.Name.LocalName == "code" && xAttribute.Value == "11450-4");
            //                                                          }) > 0
            //               select e).ToList();

            //    compList.RemoveAt(0);



            //    foreach (var xe in compList)
            //    {
            //        var codes = (from e in xe.Descendants()
            //                    where e.Name.LocalName == "entry"
            //                    select e).ToList();

            //        Console.WriteLine("test");
            //    }

            //    Console.WriteLine("test");
            //}



            //foreach (var ccd in ccdList)
            //{
            //    //var xx = ccd.Descendants().Elements().Where(x => x.Name.LocalName == "table");
            //    ccd.Descendants().Elements().Where(x => x.Name.LocalName == "table").Remove();
            //}



            //var x = new RuleList();

            //foreach (var s in x.RuleObjects)
            //{
            //    var act = Activator.CreateInstance(s);

            //    Console.WriteLine(s.InvokeMember("RuleName", BindingFlags.InvokeMethod, null, act, null));
            //}

            //Console.WriteLine(x.PrimaryRule.InvokeMember("RuleName", BindingFlags.InvokeMethod, null, Activator.CreateInstance(x.PrimaryRule), null));


            ////var x = Deserialize<POCD_MT000040ClinicalDocument>(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_1.xml");

            ////Console.WriteLine(x.ToString());

            //var ccd1 = Deserialize<POCD_MT000040ClinicalDocument>(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_1.xml");
            //var ccd2 = Deserialize<POCD_MT000040ClinicalDocument>(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_2.xml");
            //var ccd3 = Deserialize<POCD_MT000040ClinicalDocument>(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_3.xml");

            //var ccdList = new List<POCD_MT000040ClinicalDocument> {ccd1, ccd2, ccd3};

            //var x = new PrimaryRule(ccdList).MasterCCD;


            Console.Read();
        }


        //static void RunCCDDedup()
        //{
        //    //Create a ccd list of XDocuments
        //    var ccdList = new List<XDocument>();

        //    ccdList.Add(XDocument.Load(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_1.xml"));
        //    ccdList.Add(XDocument.Load(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_2.xml"));
        //    ccdList.Add(XDocument.Load(@"C:\Users\Jon\Dropbox\hhic2\CCD\HHIC_CCD_3.xml"));

        //    for (int a = 0; a < 20; a++)
        //    {

        //        XDocument xMaster;

        //        var ruleList = new RuleList();

        //        using (var xRule = RuleFactory.BuildRule(Guid.NewGuid(), ruleList.PrimaryRule, ccdList, null))
        //        {
        //            xRule.Merge();
        //            xMaster = xRule.GetMasterCcd();
        //        }


        //        foreach (var i in ruleList.MergeRules)
        //        {
        //            using (var xRule = RuleFactory.BuildRule(Guid.NewGuid(), i, ccdList, xMaster))
        //            {
        //                xRule.Merge();
        //                xMaster = xRule.GetMasterCcd();
        //            }
        //        }

        //        Console.Write("Done: " + a);
        //    }
        //}

        //public static T Deserialize<T>(string pathName)
        //{
        //    using (TextReader reader = new StreamReader(pathName))
        //    {
        //        XmlSerializer serializer = new XmlSerializer(typeof(T));
        //        return (T)serializer.Deserialize(reader);
        //    }
        //}

        //public static void Serialize<T>(T value, string pathName)
        //{
        //    using (TextWriter writer = new StreamWriter(pathName))
        //    {
        //        XmlSerializer serializer = new XmlSerializer(typeof(T));
        //        serializer.Serialize(writer, value);
        //    }
        //}
    }
}
