using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;
using MergeEngine;
using Security;
using AwsInterface;

namespace RestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CcdDeDup" in code, svc and config file together.
    public class CcdDeDup : ICcdDeDup
    {
        public string TestString()
        {
            return "Worked";
        }

        public string CreateNewMergeRequest(string key, string secretKey)
        {
            try
            {
                if (!RestSecurity.Authenticate(key, secretKey) || !RestSecurity.Authorize(key, secretKey))
                    throw new UnAuthorizedAccesssException();
                return Guid.NewGuid().ToString();
            }
            catch (UnAuthorizedAccesssException)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return "Error: 1 Unauthorized";
            }
            
        }

        public string AddCcd(string key, string secretKey, string mergeId, Stream data)
        {
            string s3Key;
            try
            {
                string xmlString;
                using (var sr = new StreamReader(data))
                {
                    xmlString = sr.ReadToEnd();
                }

                s3Key = S3Interface.AddCcdRecord(Guid.Parse(mergeId), xmlString, key);
            }
            catch (UnAuthorizedAccesssException)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return "Error: 1 Unauthorized";
            }
            catch(Exception ex)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.SeeOther;
                return "Error: 2 General Error";
            }

            return s3Key;

        }

        public string MergeWithRuleSet(string key, string secretKey, string mergeId, string data)
        {
            string s3Key;
            try
            {
                string jsonString = data;
                //using (var sr = new StreamReader(data))
                //{
                //    jsonString = sr.ReadToEnd();
                //}

                s3Key = S3Interface.AddRuleSet(Guid.Parse(mergeId), jsonString, key);
            }
            catch (UnAuthorizedAccesssException)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return "Error: 1 Unauthorized";
            }
            catch (Exception)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.SeeOther;
                return "Error: 2 General Error";
            }

            return s3Key;
        }

        public string MergeWithRuleSet(string key, string secretKey, string mergeId, Stream data)
        {
            string s3Key;
            try
            {
                string jsonString;
                using (var sr = new StreamReader(data))
                {
                    jsonString = sr.ReadToEnd();
                }

                s3Key = S3Interface.AddRuleSet(Guid.Parse(mergeId), jsonString, key);
            }
            catch (UnAuthorizedAccesssException)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return "Error: 1 Unauthorized";
            }
            catch (Exception)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.SeeOther;
                return "Error: 2 General Error";
            }

            return s3Key;
        }

        public string MergeWithDefaultRules(string key, string secretKey, string mergeId)
        {
            string s3Key;
            try
            {
                const string jsonString = "{\"UseDefault\": \"true\"}";

                s3Key = S3Interface.AddRuleSet(Guid.Parse(mergeId), jsonString, key, true);
            }
            catch (UnAuthorizedAccesssException)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return "Error: 1 Unauthorized";
            }
            catch (Exception)
            {
                var ctx = WebOperationContext.Current;
                if (ctx != null) ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.SeeOther;
                return "Error: 2 General Error";
            }

            return s3Key;
        }

        public string MergeStatus(string key, string secretKey, string mergeId)
        {
            return S3Interface.GetMergeStatus(Guid.Parse(mergeId));
        }

        public Stream GetMergedCcd(string key, string secretKey, string mergeId)
        {
            return S3Interface.GetMergeSet(Guid.Parse(mergeId), key);
        }

        //TESTING/DEMO ONLY!!!
        public string FireMergeEngine()
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

            return string.Empty;
        }
    }
}
