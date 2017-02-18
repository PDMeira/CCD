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
using System.Threading;
using System.Xml.Linq;
using MergeEngine;
using CcdInterfaces;

namespace BatchProcessTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a ccd list of XDocuments
            var ccdList = new List<XDocument>();
            //var ruleList = new RuleList();
            var ruleSet = DefaultRuleset.GetDefaultRuleset();

            ccdList.Add(XDocument.Load(@"https://s3.amazonaws.com/hhicccddemo/HHIC_CCD_1.xml"));
            ccdList.Add(XDocument.Load(@"https://s3.amazonaws.com/hhicccddemo/HHIC_CCD_2.xml"));
            ccdList.Add(XDocument.Load(@"https://s3.amazonaws.com/hhicccddemo/HHIC_CCD_3.xml"));

            for (int i = 0; i < 25; i++)
            {
                
                //ThreadPool.QueueUserWorkItem(new WaitCallback(runMerge));
                
                runMerge(ccdList, ruleSet, Guid.NewGuid());
                
            }

            Console.Read();
        }

        static void runMerge(List<XDocument> ccdList, RuleSet ruleSet, Guid mergeId)
        {
            try
            {
                Console.WriteLine("Merge Starting...");
                var startTime = DateTime.Now;

                

                var xMaster = XDocument.Parse("<empty/>");

                foreach (var i in ruleSet.PreFormatRules)
                {
                    using (var xRule = RuleFactory.BuildPreFormatRule(mergeId, i.RuleName, i.RuleVersion , ccdList))
                    {
                        xRule.Merge();
                    }
                }

                using (var xRule = RuleFactory.BuildPrimaryRule(mergeId, ruleSet.PrimaryRule.RuleName, ruleSet.PrimaryRule.RuleVersion, ccdList))
                {
                    xRule.Merge();
                    xMaster = xRule.GetMasterCcd();
                }


                foreach (var i in ruleSet.DeDuplicationRules)
                {
                    using (var xRule = RuleFactory.BuildDeDupRule(mergeId, i.RuleName, i.RuleVersion, ccdList, xMaster))
                    {
                        xRule.Merge();
                        xMaster = xRule.GetMasterCcd();
                    }
                }

                foreach (var i in ruleSet.PostFormatRules)
                {
                    using (var xRule = RuleFactory.BuildPostFormatRule(mergeId, i.RuleName, i.RuleVersion, xMaster))
                    {
                        xRule.Merge();
                        xMaster = xRule.GetMasterCcd();
                    }
                }
                var runTime = DateTime.Now - startTime;
                Console.WriteLine("Merge Complete in: " + runTime.TotalSeconds.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("Error In Merge");
            }
            
        }
        
    }
}
