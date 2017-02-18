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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Data;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

namespace Audit
{
   
    public class AuditRecordMongo
    {
        public ObjectId Id { get; set; }
        public Guid AuditId { get; set; }
        public Guid MergeId { get; set; }
        public DateTime DateStamp { get; set; }
        public string MergeRule { get; set; }
        public int RuleVersion { get; set; }
        [BsonIgnore]
        public List<XDocument> PreRuleCcdList { get; set; }
        [BsonIgnore]
        public List<XDocument> PostRuleCcdList { get; set; }
        [BsonIgnore]
        public XDocument PreRuleMasterCcd { get; set; }
        [BsonIgnore]
        public XDocument PostRuleMasterCcd { get; set; }
        [BsonIgnore]
        public List<XElement> DiscardData { get; set; }
        public double RunSeconds { get; set; }

        
        public List<string> PreRuleCcdListStrings
        {
            get { return PreRuleCcdList == null? new List<string>() : PreRuleCcdList.Select(x => x.ToString()).ToList(); }
            set { PreRuleCcdList.AddRange(value.Select(x => XDocument.Parse(x)).ToList());}
        }

        
        public List<string> PostRuleCcdListStrings
        {
            get { return PostRuleCcdList == null ? new List<string>() : PostRuleCcdList.Select(x => x.ToString()).ToList(); }
            set { PostRuleCcdList.AddRange(value.Select(XDocument.Parse).ToList()); }
        }

        
        public string PreRuleMasterCcdString
        {
            get { return PreRuleMasterCcd == null ? "<empty/>" : PreRuleMasterCcd.ToString(); }
            set { PreRuleMasterCcd = XDocument.Parse(value); }
        }

        
        public string PostRuleMasterCcdString
        {
            get { return PostRuleMasterCcd == null ? "<empty/>" : PostRuleMasterCcd.ToString(); }
            set { PostRuleMasterCcd = XDocument.Parse(value); }
        }

        
        public List<string> DiscardDataStrings
        {
            get { return DiscardData == null? new List<string>() : DiscardData.Select(x => x.ToString()).ToList(); }
            set { DiscardData.AddRange(value.Select(XElement.Parse).ToList()); }
        }

        public AuditRecordMongo()
        {
            
        }

        public AuditRecordMongo(Guid mergeId, string mergeRule, int ruleVersion, List<XDocument> preRuleCcdList, XDocument preRuleMasterCcd)
        {
            AuditId = Guid.NewGuid();
            MergeId = mergeId;
            DateStamp = DateTime.Now;
            MergeRule = mergeRule;
            RuleVersion = ruleVersion;
            PreRuleCcdList = preRuleCcdList;
            PostRuleCcdList = new List<XDocument>();
            PreRuleMasterCcd = preRuleMasterCcd;
            DiscardData = new List<XElement>();
        }

        public AuditRecordMongo(AuditRecord auditRecord)
        {
            AuditId = auditRecord.AuditId;
            MergeId = auditRecord.MergeId;
            DateStamp = auditRecord.DateStamp;
            MergeRule = auditRecord.MergeRule;
            RuleVersion = auditRecord.RuleVersion;
            PreRuleCcdList = auditRecord.PreRuleCcdList;
            PostRuleCcdList = auditRecord.PostRuleCcdList;
            PreRuleMasterCcd = auditRecord.PreRuleMasterCcd;
            PostRuleMasterCcd = auditRecord.PostRuleMasterCcd;
            RunSeconds = auditRecord.RunSeconds;
        }

        public void Complete(List<XDocument> ccdList, XDocument masterCcd, List<XElement> discardData)
        {
            PostRuleCcdList = ccdList;
            PostRuleMasterCcd = masterCcd;
            DiscardData = discardData;
            RunSeconds = (DateTime.Now - DateStamp).TotalMilliseconds/1000;

        }

        public string ToBson()
        {
            return string.Empty;
        }
        
        

        public static XmlNode ToXmlNode(XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

    }
}