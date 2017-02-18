using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using MergeEngine;
using Ionic.Zip;
using System.Diagnostics;

namespace TestAppLocal
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Duplicate();
            De_Duplicate();
            UnzipFile();
            Analyze();
        }
        public static string folderPath = @"C:\Users\mhosseini\";
        static void Duplicate()
        {
            string folderPath = @"C:\Users\mhosseini";
            foreach (string file in Directory.GetFiles(folderPath, "*.xml"))
            {
                NewDuplicatedCCD doc1 = new NewDuplicatedCCD(10, file);
                NewDuplicatedCCD doc2 = new NewDuplicatedCCD(5, file);
                XDocument mainCcd = XDocument.Load(file);


                var fileName = Path.GetFileName(file);
                fileName = fileName.Remove(fileName.Length - 4);

                var dir = @"C:\Users\mhosseini\" + fileName;  // folder location
                if (!Directory.Exists(dir))  // if it doesn't exist, create
                    Directory.CreateDirectory(dir);

                mainCcd.Save(dir + @"\" + fileName + "0.xml");
                doc1.Doc.Save(dir + @"\" + fileName + "1.xml");
                doc2.Doc.Save(dir + @"\" + fileName + "2.xml");
            }
        }
        static void De_Duplicate()
        {

            //Create a ccd list of XDocuments           



            Stopwatch sw = new Stopwatch();
            RuleSet ruleSet;
            ruleSet = new RuleSet();
            // ruleSet.AddRule("StripText", 0, RuleType.PreFormat);
            ruleSet.AddRule("PrimaryMergeRuleWithValidation", 0, RuleType.Primary);
            ruleSet.AddRule("Author", 0, RuleType.DeDuplication);
            ruleSet.AddRule("Confidentiality", 0, RuleType.DeDuplication);
            ruleSet.AddRule("MedicationRule1", 0, RuleType.DeDuplication);
            ruleSet.AddRule("AlertsSection", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("SocialHistory_DailyCaffeine", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("SocialHistory_EtohUser", 0, RuleType.DeDuplication);
            //ruleSet.AddRule("SocialHistory_SmokingConsolidation", 0, RuleType.DeDuplication);
            ruleSet.AddRule("VitalSigns", 0, RuleType.DeDuplication);
            ruleSet.AddRule("TestProblemsDedup", 0, RuleType.DeDuplication);
            // ruleSet.AddRule("AddFormatedText", 0, RuleType.PostFormat);

            string guid = "{3F2504E0-4F89-41D3-9A0C-0305E82C3301}";

      
            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                string fileName = Path.GetFileName(folder);

                var ccdList = new List<XDocument>();
                foreach (string file in Directory.GetFiles(folder))
                {
                    ccdList.Add(XDocument.Load(file));
                }


                
                sw.Start();
                Stream ms = RunMerge.runMergeWithRuleSet(ccdList, ruleSet, Guid.Parse(guid), fileName);
                sw.Stop();
            }
            Console.WriteLine("------------------ Consolidation is done! --------------------------");

            Console.WriteLine("CountDedup.txt", "Vital Count: " + DeDupRule.vitalDedupCount);
            Console.WriteLine("Problem Count: " + DeDupRule.problemsDedupCount);
            Console.WriteLine("Alert Count: " + DeDupRule.alertDedupCount);
            Console.WriteLine("Medication Count: " + DeDupRule.medicationDedupCount);
            Console.WriteLine("Confidentiality Count: " + DeDupRule.confidentialityDedupCount);
            Console.WriteLine("Author Count: " + DeDupRule.authorDedupCount);
            Console.WriteLine("Overall deduplication time: " + sw.Elapsed);


            Console.ReadKey();



        }
        static void Analyze()
        {
            List<CCDsGroupInfo> MainCcdGroup = new List<CCDsGroupInfo>();
      
            int counter = 0;
            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                CCDsGroupInfo ccd4Group = new CCDsGroupInfo();
                ccd4Group.ccdList = new List<CCDinfo>();
                foreach (string file in Directory.GetFiles(folder))
                {
                    if (Path.GetExtension(file) != ".xml")
                        continue;
                    XDocument randDoc = XDocument.Load(file);
                    int problemCount = (from e in GetSectionByCode(randDoc, "11450-4").Descendants()
                                        where e.Name.LocalName == "entry"
                                        select e).ToList().Count;

                    int medicationCount = (from e in GetSectionByCode(randDoc, "10160-0").Descendants()
                                           where e.Name.LocalName == "entry"
                                           select e).ToList().Count;

                    int allergyCount = (from e in GetSectionByCode(randDoc, "48765-2").Descendants()
                                        where e.Name.LocalName == "entry"
                                        select e).ToList().Count;
                    var ccdFileName = Path.GetFileName(file);
                    ccdFileName = ccdFileName.Remove(ccdFileName.Length - 4);
                    FileInfo fi = new FileInfo(file);
                    long ccdSize = fi.Length / 1024;
                    int ccdLines = File.ReadAllLines(file).Length;

                    ccd4Group.ccdList.Add(new CCDinfo() { fileName = ccdFileName, probCount = problemCount, medCount = medicationCount, alleCount = allergyCount, 
                                                        fileSize = ccdSize, lineCount = ccdLines });
                }
                counter++;
                ccd4Group.groupName = "Group " + counter;
                MainCcdGroup.Add(ccd4Group);
            }

            //string data = String.Empty;
            //for (int col = 0; col < datagridview.ColumnCount; col++)
            //{
            //    data += Convert.ToString(datagridview.Columns[col].HeaderText);
            //}
            //data += "\n";
            //for (int i = 0; i < datagridview.RowCount; i++)
            //{
            //    for (int j = 0; j < datagridview.ColumnCount; j++)
            //    {
            //        data += Convert.ToString(datagridview.Rows[i].Cells[j].Value);
            //        data += "\t";
            //    }
            //    data += "\n";
            //}





            string row = String.Empty;
            row += "Group Name \t CCD Name \t Medications Count \t Problems Count \t Allergies Count \t File Size \t Lines count \n\n";
            int rowCounter = 0;
            MainCcdGroup.ForEach(x =>
                {
                    row += x.groupName + "\t";

                    x.ccdList.ForEach(y =>
                     {
                         if (rowCounter == 0)
                             row += y.fileName + "\t" + y.medCount + "\t" + y.probCount + "\t" + y.alleCount + "\t" + y.fileSize + "\t" + y.lineCount + "\n";
                         else
                             row += "\t" + y.fileName + "\t" + y.medCount + "\t" + y.probCount + "\t" + y.alleCount + "\t" + y.fileSize + "\t" + y.lineCount + "\n";
                         rowCounter++;
                     });
                    rowCounter = 0;
                    row += "\n \n";
                });




            StreamWriter SW;
            SW = File.AppendText(folderPath + "\\result.txt");
            SW.WriteLine(row);
            SW.Close();
        }
        static void UnzipFile()
        {
     
            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    if (Path.GetExtension(file) == ".zip")
                    {
                        using (ZipFile zip = ZipFile.Read(file))
                        {
                            foreach (ZipEntry e in zip)
                            {
                                if (e.FileName == "MergedCcd.xml")
                                    e.Extract(folder);
                            }

                        }
                    }
                }
            }
        }
        static XElement GetSectionByCode(XDocument ccd, string code)
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
    }

    class NewDuplicatedCCD
    {
        XDocument doc;
        public XDocument Doc
        {
            get { return doc; }
            set { doc = value; }
        }
        public NewDuplicatedCCD(int addMax, string filePath)
        {
            XDocument randDoc = XDocument.Load(@"C:\Users\mhosseini\Randomization.xml");
            var extraProb = (from e in randDoc.Elements().Descendants()
                             where e.Parent.Name.LocalName == "Problems"
                             select e).ToList();

            var extraMed = (from e in randDoc.Elements().Descendants()
                            where e.Parent.Name.LocalName == "Medication"
                            select e).ToList();

            var extraAllergy = (from e in randDoc.Elements().Descendants()
                                where e.Parent.Name.LocalName == "Allergy"
                                select e).ToList();


            // doc = XDocument.Load(filePath);


            Doc = Manipulation(XDocument.Load(filePath), extraProb, extraMed, extraAllergy, addMax);

        }

        static XElement GetSectionByCode(XDocument ccd, string code)
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

        static void MergeToMaster(XDocument MasterCcd, List<XElement> elements, string code)
        {

            XComment MergeMessage = new XComment("Merged Information By Randomization Engine");

            //removes elements from the section

            MasterCcd.Descendants().Elements().Last(x => x.Name.LocalName == "section" && x.Elements().Count(y =>
            {
                var yAttribute = y.Attribute("code");
                return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == code);
            }) > 0).Elements().Where(x => x.Name.LocalName == "entry").Remove();

            //Adds each element in the list to the master ccd
            foreach (var i in elements)
            {
                i.AddFirst(MergeMessage);

                MasterCcd.Descendants().Last(x => x.Name.LocalName == "section"
                && x.Elements().Count(y =>
                {
                    var yAttribute = y.Attribute("code");
                    return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == code);
                }) > 0).Add(i);
            }
        }

        static XDocument Manipulation(XDocument ccd, List<XElement> extraProb, List<XElement> extraMed, List<XElement> extraAllergy, int addMax)
        {
            var probEntries = (from e in GetSectionByCode(ccd, "11450-4").Descendants()
                               where e.Name.LocalName == "entry"
                               select e).ToList();
            var medEntries = (from e in GetSectionByCode(ccd, "10160-0").Descendants()
                              where e.Name.LocalName == "entry"
                              select e).ToList();
            var allertEntries = (from e in GetSectionByCode(ccd, "48765-2").Descendants()
                                 where e.Name.LocalName == "entry"
                                 select e).ToList();

            Random rnd = new Random();
            probEntries.RemoveRange(0, rnd.Next(1, probEntries.Count));
            probEntries.AddRange(extraProb.GetRange(0, rnd.Next(1, addMax)));


            medEntries.RemoveRange(0, rnd.Next(1, medEntries.Count));
            medEntries.AddRange(extraMed.GetRange(0, rnd.Next(1, addMax)));

            allertEntries.RemoveRange(0, rnd.Next(1, allertEntries.Count));
            allertEntries.AddRange(extraAllergy.GetRange(0, rnd.Next(1, addMax)));


            MergeToMaster(ccd, probEntries, "11450-4");
            MergeToMaster(ccd, medEntries, "10160-0");
            MergeToMaster(ccd, allertEntries, "48765-2");

            //MergeEngine.DeDupRule.tttddd = 1;

            return ccd;

        }

    }



    public class CCDsGroupInfo
    {
        public string groupName;
        public List<CCDinfo> ccdList;

    }

    public class CCDinfo
    {
        public string fileName;
        public int probCount;
        public int medCount;
        public int alleCount;
        public long fileSize;
        public int lineCount;
    }
}
