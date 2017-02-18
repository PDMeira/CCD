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
using System.Xml;
using System.Xml.Linq;
using CcdInterfaces;
using Data;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System.Configuration;
using Newtonsoft.Json;
using CustomExceptions;

namespace Audit
{
    public enum PrePost
    {
        Pre = 0,
        Post = 1
    }

    
    public class AuditRecord 
    {
        public Guid AuditId { get; set; }
        public Guid MergeId { get; set; }
        public DateTime DateStamp { get; set; }
        public string MergeRule { get; set; }
        public int RuleVersion { get; set; }
        [JsonIgnore]
        public List<XDocument> PreRuleCcdList { get; set; }
        [JsonIgnore]
        public List<XDocument> PostRuleCcdList { get; set; }
        [JsonIgnore]
        public XDocument PreRuleMasterCcd { get; set; }
        [JsonIgnore]
        public XDocument PostRuleMasterCcd { get; set; }
        [JsonIgnore]
        public List<XElement> DiscardData { get; set; }
        public double RunSeconds { get; set; }

        public List<string> PreRuleCcdListJson
        {
            get { return PreRuleCcdList.Select(x => JsonConvert.SerializeXmlNode(ToXmlNode(x.Root))).ToList(); }
        }

        public List<string> PostRuleCcdListJson
        {
            get { return PostRuleCcdList.Select(x => JsonConvert.SerializeXmlNode(ToXmlNode(x.Root))).ToList(); }
        }

        public string PreRuleMasterCcdJson
        {
            get { return JsonConvert.SerializeXmlNode(ToXmlNode(PreRuleMasterCcd == null ? XElement.Parse("<empty/>") : PreRuleMasterCcd.Root)); }
        }

        public string PostRuleMasterCcdJson
        {
            get { return JsonConvert.SerializeXmlNode(ToXmlNode(PostRuleMasterCcd == null ? XElement.Parse("<empty/>") : PostRuleMasterCcd.Root)); }
        }

        public List<string> DiscardDataJson
        {
            get { return DiscardData.Select(x => JsonConvert.SerializeXmlNode(ToXmlNode(x))).ToList(); }
        }

        public AuditRecord(object ruleObject, Guid mergeId)
        {
            AuditId = Guid.NewGuid();
            MergeId = mergeId;
            DateStamp = DateTime.Now;

            var intf = ruleObject.GetType().GetInterfaces().FirstOrDefault(x => x.GetInterfaces().Select(y => y.Name).Contains("IRule"));
            
            if (intf != null)
                switch (intf.Name)
                {
                    case "IPreFormatRule":
                        var preFormatRule = (IPreFormatRule) ruleObject;
                        MergeRule = preFormatRule.GetType().Name;
                        RuleVersion = preFormatRule.RuleVersion();
                        PreRuleCcdList = preFormatRule.GetCcdList();
                        PostRuleCcdList = new List<XDocument>();
                        DiscardData = new List<XElement>();
                        break;
                    case "IPrimaryRule":
                        var primaryRule = (IPrimaryRule) ruleObject;
                        MergeRule = primaryRule.GetType().Name;
                        RuleVersion = primaryRule.RuleVersion();
                        PreRuleCcdList = primaryRule.GetCcdList();
                        PostRuleCcdList = new List<XDocument>();
                        DiscardData = new List<XElement>();
                        break;
                    case "IDeDupRule":
                        var deDupRule = (IDeDupRule)ruleObject;
                        MergeRule = deDupRule.GetType().Name;
                        RuleVersion = deDupRule.RuleVersion();
                        PreRuleCcdList = deDupRule.GetCcdList();
                        PostRuleCcdList = new List<XDocument>();
                        DiscardData = new List<XElement>();
                        break;
                    case "IPostFormatRule":
                        var postFormatRule = (IPostFormatRule)ruleObject;
                        MergeRule = postFormatRule.GetType().Name;
                        RuleVersion = postFormatRule.RuleVersion();
                        PreRuleMasterCcd = postFormatRule.GetMasterCcd();
                        PreRuleCcdList = new List<XDocument>();
                        PostRuleCcdList = new List<XDocument>();
                        DiscardData = new List<XElement>();
                        break;
                    default:
                        throw new InvalidRuleObjectException();
                        break;
                }

            


            
        }

        [System.Obsolete("Use Rule Object Constructor")]
        public AuditRecord(Guid mergeId, string mergeRule, int ruleVersion, List<XDocument> preRuleCcdList, XDocument preRuleMasterCcd)
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

        public void Complete(object ruleObject)
        {
            var intf = ruleObject.GetType().GetInterfaces().FirstOrDefault(x => x.GetInterfaces().Select(y => y.Name).Contains("IRule"));
            
            if (intf != null)
                switch (intf.Name)
                {
                    case "IPreFormatRule":
                        var preFormatRule = (IPreFormatRule)ruleObject;
                        PostRuleCcdList = preFormatRule.GetCcdList();
                        DiscardData = preFormatRule.GetDiscarded();
                        break;
                    case "IPrimaryRule":
                        var primaryRule = (IPrimaryRule)ruleObject;
                        PostRuleMasterCcd = primaryRule.GetMasterCcd();
                        break;
                    case "IDeDupRule":
                        var deDupRule = (IDeDupRule)ruleObject;
                        PostRuleMasterCcd = deDupRule.GetMasterCcd();
                        DiscardData = deDupRule.GetDiscarded();
                        break;
                    case "IPostFormatRule":
                        var postFormatRule = (IPostFormatRule)ruleObject;
                        PostRuleMasterCcd = postFormatRule.GetMasterCcd();
                        break;
                    default:
                        throw new InvalidRuleObjectException();
                        break;
                }

            RunSeconds = (DateTime.Now - DateStamp).TotalMilliseconds / 1000;

        }

        [System.Obsolete("Use Rule Object Method")]
        public void Complete(List<XDocument> ccdList, XDocument masterCcd, List<XElement> discardData)
        {
            PostRuleCcdList = ccdList;
            PostRuleMasterCcd = masterCcd;
            DiscardData = discardData;
            RunSeconds = (DateTime.Now - DateStamp).TotalMilliseconds/1000;

        }


        public string ToJson()
        {
            return "{ objJson: " + JsonConvert.SerializeObject(this) + "}";
        }


        public void SaveDynamoDb()
        {
            try
            {
                var config = new AmazonDynamoDBConfig();
                config.ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"];
                var client = new AmazonDynamoDBClient(config);

                Table ccdAudit = Table.LoadTable(client, "CcdAudit");

                var Audit = new Document();

                Audit["AuditId"] = AuditId.ToString();
                Audit["MergeId"] = MergeId.ToString();
                Audit["DateStamp"] = DateStamp;
                Audit["MergeRule"] = MergeRule;
                Audit["RuleVersion"] = RuleVersion;
                Audit["PreRuleMasterCcd"] = PreRuleMasterCcd.ToString();
                Audit["PostRuleMasterCcd"] = PostRuleMasterCcd.ToString();
                Audit["RunSeconds"] = RunSeconds;

                ccdAudit.PutItem(Audit);

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void SaveRuleList()
        {
            using (var db = new ccddedupDataContext())
            {
                if (PreRuleCcdList != null && PreRuleCcdList.Count != 0)
                    db.audit_ccds.InsertAllOnSubmit(PreRuleCcdList.Select(x => new audit_ccd()
                    {
                        auditId = AuditId,
                        CcdData = x.Root,
                        PrePost = (int)PrePost.Pre
                    }));
                if (PreRuleCcdList != null && PostRuleCcdList.Count != 0)
                    db.audit_ccds.InsertAllOnSubmit(PostRuleCcdList.Select(x => new audit_ccd()
                    {
                        auditId = AuditId,
                        CcdData = x.Root,
                        PrePost = (int)PrePost.Post
                    }));

                db.SubmitChanges();
            }
        }

        public void SaveDiscard()
        {
            using (var db = new ccddedupDataContext())
            {
                if (DiscardData != null)
                    db.audit_discards.InsertAllOnSubmit(DiscardData.Select(x => new audit_discard()
                    {
                        AuditId = AuditId,
                        DiscardData = x
                    }));

                db.SubmitChanges();
            }
        }

        public void Save()
        {
            using (var db = new ccddedupDataContext())
            {
                db.audits.InsertOnSubmit(new audit()
                {
                    AuditId = AuditId,
                    MergeId = MergeId,
                    DateStamp = DateStamp,
                    MergeRule = MergeRule,
                    RuleVersion = RuleVersion,
                    PreRuleMasterCcd = PreRuleMasterCcd == null ? XElement.Parse("<empty/>") : PreRuleMasterCcd.Root,
                    PostRuleMasterCcd = PostRuleMasterCcd == null ? XElement.Parse("<empty/>") : PostRuleMasterCcd.Root,
                    RunSeconds = RunSeconds
                });

                db.SubmitChanges();

            }
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
