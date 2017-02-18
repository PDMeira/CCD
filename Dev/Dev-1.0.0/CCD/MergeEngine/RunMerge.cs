using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Audit;
using Ionic.Zip;



namespace MergeEngine
{
    public static class RunMerge
    {
        public static Stream runMergeWithRuleSet(List<XDocument> ccdList, RuleSet ruleSet, Guid mergeId, string fileName)
        {
            var xMaster = XDocument.Parse("<empty/>");
            var auditRecords = new List<string>();
            

            foreach (var i in ruleSet.PreFormatRules)
            {
                try
                {
                    using (var xRule = RuleFactory.BuildPreFormatRule(mergeId, i.RuleName, i.RuleVersion, ccdList))
                    {
                        xRule.Merge();
                        ccdList = xRule.GetCcdList();
                        auditRecords.Add(xRule.GetAuditRecordBson());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            using (var xRule = RuleFactory.BuildPrimaryRule(mergeId, ruleSet.PrimaryRule.RuleName, ruleSet.PrimaryRule.RuleVersion, ccdList))
            {
                try
                {
                    xRule.Merge();
                    xMaster = xRule.GetMasterCcd();
                    auditRecords.Add(xRule.GetAuditRecordBson());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            foreach (var i in ruleSet.DeDuplicationRules)
            {
                try
                {
                    using (var xRule = RuleFactory.BuildDeDupRule(mergeId, i.RuleName, i.RuleVersion, ccdList, xMaster))
                    {
                        xRule.Merge();
                        xMaster = xRule.GetMasterCcd();


                        auditRecords.Add(xRule.GetAuditRecordBson());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            foreach (var i in ruleSet.PostFormatRules)
            {
                try
                {
                    using (var xRule = RuleFactory.BuildPostFormatRule(mergeId, i.RuleName, i.RuleVersion, xMaster))
                    {
                        xRule.Merge();
                        xMaster = xRule.GetMasterCcd();
                        auditRecords.Add(xRule.GetAuditRecordBson());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
           

            using (var zip = new ZipFile())
            {
                zip.AddEntry("MergedCcd.xml", xMaster.ToString());

                for (var i = 0; i < auditRecords.Count; i++)
                {
                    zip.AddEntry("AuditRecord_" + i + ".txt", auditRecords[i]);
                }

                for (var i = 0; i < ccdList.Count; i++)
                {
                    zip.AddEntry("CCD_" + i + ".xml", ccdList[i].ToString());
                }

                

                var outputStream = new MemoryStream();

                string result = Path.GetRandomFileName();
                result += result + ".zip";

                string folderPath = @"C:\Users\mhosseini\My Box Files\Default Sync Folder\_REGENSTRIEF\De-Dupit\Article\SSACCDresult\RandDocs\";

                zip.Save(folderPath + fileName + "\\" + result);
                //zip.Save(outputStream);
                return outputStream;
            }
        }
    }
}
