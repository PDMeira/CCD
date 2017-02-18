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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CcdInterfaces;

namespace MergeEngine
{
    /// <summary>
    /// Helper class for medications.  Holds the elements and possible dedup data
    /// </summary>
    class MedicationRule1_Entry
    {
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string OriginalText { get; set; }
        public string Form { get; set; }
        public string Code { get; set; }
        public string CodeSystem { get; set; }
        public string Provider { get; set; }
        public XElement Element { get; set; }

    }

    /// <summary>
    /// Class to remove duplicats from the medication section
    /// </summary>
    public class MedicationRule1 : DeDupRule, IDeDupRule
    {
        private List<MedicationRule1_Entry> _medicationRule1Entries;

        public MedicationRule1()
        {
            _medicationRule1Entries = new List<MedicationRule1_Entry>();
        }
        public string RuleName()
        {
            return "Test Medication Rule 1";
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
            //Add the passed values to object
            BuildMedicationList();

           // RemoveInvalidEntries();
            RemoveExactDuplicates();
            RemoveDuplicatesByCodeAndProvider();

            MergeToMaster(_medicationRule1Entries.Select(x => x.Element).ToList(), "10160-0");
        }

        //public override XDocument MergeCCDs(List<XDocument> ccdList, XDocument masterCcd)
        //{
        //    //Add the passed values to object
        //    CcdList = ccdList;
        //    MasterCcd = masterCcd;

        //    BuildMedicationList();

        //    RemoveInvalidEntries();
        //    RemoveExactDuplicates();
        //    RemoveDuplicatesByCodeAndProvider();

        //    //_masterCcd.Descendants().Last(x => x.Name.LocalName == "section"
        //    //    && x.Elements().Count(y =>
        //    //    {
        //    //        var yAttribute = y.Attribute("code");
        //    //        return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == "10160-0");
        //    //    }) > 0).Descendants().Elements().Where(x => x.Name.LocalName == "entry").Remove();

        //    //foreach (var i in _medicationRule1Entries)
        //    //{
        //    //    masterCcd.Descendants().Last(x => x.Name.LocalName == "section"
        //    //    && x.Elements().Count(y =>
        //    //    {
        //    //        var yAttribute = y.Attribute("code");
        //    //        return yAttribute != null && (y.Name.LocalName == "code" && yAttribute.Value == "10160-0");
        //    //    }) > 0).Add(i.Element);
        //    //}

        //    return MergeToMaster(MasterCcd, _medicationRule1Entries.Select(x => x.Element).ToList(), "10160-0");
        //}

        /// <summary>
        /// Extracts the medication elements and the possible info for dedup
        /// </summary>
        private void BuildMedicationList()
        {
            foreach (var i in CcdList)
            {
                //TemplateId for medication is 2.16.840.1.113883.10.20.1.8
                var section = GetSectionByCode(i, "10160-0");

                var entries = (from e in section.Descendants()
                               where e.Name.LocalName == "entry"
                               select e).ToList();

                foreach (var e in entries)
                {
                    //Get Med Date
                    var startDate = new DateTime?();
                    string[] dateTimeFormats = new string[] { "yyyyMMddHHmmss.fffzzz", "yyyyMMddHHmmsszzz", "yyyyMMdd" };
                    try //<-- The try catch here is not ideal but saves a lot of time handling the null values from the xml
                    {
                        var aStartDate = e.Descendants().Elements().FirstOrDefault(x =>
                        {
                            var xAttribute = x.Attribute(ns + "type");
                            return xAttribute != null && (x.Name.LocalName == "effectiveTime" && xAttribute.Value == "IVL_TS");
                        }).Descendants().FirstOrDefault(x => x.Name.LocalName == "low").Attribute("value");

                        if (aStartDate != null)
                        {
                            startDate = DateTime.ParseExact(aStartDate.Value, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        }
                    }
                    catch (Exception)
                    {
                        startDate = null;
                    }


                    //Get Status
                    var statusCode = "";
                    try
                    {
                        var aStatusCode = e.Descendants().Elements().FirstOrDefault(x => x.Name.LocalName == "statusCode").Attribute("code");

                        if (aStatusCode != null)
                        {
                            statusCode = aStatusCode.Value;
                        }
                    }
                    catch (Exception)
                    {
                        statusCode = "UNK";
                    }


                    //Get Form
                    var form = "";
                    try
                    {
                        var aForm =
                        e.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration").Elements().FirstOrDefault(
                            x => x.Name.LocalName == "administrationUnitCode").Attribute("displayName");

                        if (aForm != null)
                            form = aForm.Value;
                    }
                    catch (Exception)
                    {
                        form = "UNK";
                    }


                    //Get Drug info
                    XElement drugInfo;
                    try
                    {
                        drugInfo = e.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "consumable")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "manufacturedProduct")
                        .Elements().FirstOrDefault(x => x.Name.LocalName == "manufacturedMaterial");
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    var medCode = "";
                    var medCodeSystem = "";

                    try
                    {
                        var aMedCode = drugInfo.Elements().FirstOrDefault(x => x.Name.LocalName == "code").Attribute("code");
                        var aMedCodeSystem = drugInfo.Elements().FirstOrDefault(x => x.Name.LocalName == "code").Attribute("codeSystemName");

                        if (aMedCode != null && aMedCodeSystem != null)
                        {
                            medCode = aMedCode.Value;
                            medCodeSystem = aMedCodeSystem.Value;
                        }
                    }
                    catch (Exception)
                    {
                        medCode = "UNK";
                        medCodeSystem = "UNK";
                    }


                    var origText = "";
                    try
                    {
                        origText = drugInfo.Descendants().Elements().FirstOrDefault(x => x.Name.LocalName == "originalText").Value;
                    }
                    catch (Exception)
                    {
                        origText = "UNK";
                    }

                    var name = "";
                    try
                    {
                        name = drugInfo.Elements().FirstOrDefault(x => x.Name.LocalName == "name").Value;
                    }
                    catch (Exception)
                    {
                        name = "UNK";
                    }


                    //Get Provider as element, should be the same accross ccd
                    var provider = "";
                    try
                    {
                        provider = e.Elements().FirstOrDefault(x => x.Name.LocalName == "substanceAdministration")
                            .Elements().FirstOrDefault(x =>
                                                           {
                                                               var attribute = x.Attribute("typeCode");
                                                               return attribute != null &&
                                                                      (x.Name.LocalName == "entryRelationship" &&
                                                                       attribute.Value == "REFR");
                                                           }).Descendants().Elements().FirstOrDefault(
                                                               x => x.Name.LocalName == "name").Value;
                    }
                    catch (Exception)
                    {
                        provider = "";
                    }

                    _medicationRule1Entries.Add(new MedicationRule1_Entry()
                    {
                        Code = medCode,
                        CodeSystem = medCodeSystem,
                        Date = startDate,
                        Form = form,
                        Name = name,
                        OriginalText = origText,
                        Status = statusCode,
                        Provider = provider,
                        Element = e
                    });


                }
            }
        }

        
        /// <summary>
        /// In CCD 3 there is one section that has a mismatch on the name and original text.  The name is 
        /// nexium and the original text is some sort of hormone replacment drug.
        /// </summary>
        private void RemoveInvalidEntries()
        {
            var newMedList = _medicationRule1Entries.Where(x => x.Name == x.OriginalText).ToList();

            Discarded.AddRange(_medicationRule1Entries.Where(x => !newMedList.Contains(x)).Select(x => x.Element));

            _medicationRule1Entries = newMedList;
        }


        /// <summary>
        /// This code removes any exact duplicates from the medication list.  
        /// These are unlikely to exist.
        /// </summary>
        private void RemoveExactDuplicates()
        {
            var newMedList = _medicationRule1Entries.Distinct().ToList();

            Discarded.AddRange(_medicationRule1Entries.Where(x => !newMedList.Contains(x)).Select(x => x.Element));

            _medicationRule1Entries = newMedList;

        }

        /// <summary>
        /// This code uses the RXNORM code plus the provider to remove duplication
        /// </summary>
        private void RemoveDuplicatesByCodeAndProvider()
        {
            for (int i = _medicationRule1Entries.Count - 1; i > -1; i--)
            {
                if (_medicationRule1Entries.Count(x => x.Code == _medicationRule1Entries[i].Code
                    && x.CodeSystem == _medicationRule1Entries[i].CodeSystem
                    && x.Provider == _medicationRule1Entries[i].Provider) == 1 || _medicationRule1Entries[i].Code == "")
                    continue;

                var xcount = _medicationRule1Entries.Count(x => x.Code == _medicationRule1Entries[i].Code
                                                                && x.CodeSystem == _medicationRule1Entries[i].CodeSystem
                                                                && x.Provider == _medicationRule1Entries[i].Provider);

                Discarded.Add(_medicationRule1Entries[i].Element);
                _medicationRule1Entries.RemoveAt(i);
                medicationDedupCount++;// counting overall number of deduplation 

            }
        }



    }
}
