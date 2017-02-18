using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MergeEngine
{
    public class RuleDefinition
    {
        public string RuleName { get; set; }
        public int RuleVersion { get; set; }
        public RuleType RuleType { get; set; }
    }

    /// <summary>
    /// RuleSet class is used to create a JSON object and read a JSON object that holds different rules to be used to run a CCD Merge
    /// </summary>
    public class RuleSet
    {
        private List<RuleDefinition> _completeRuleSet;

        
        public List<RuleDefinition> PreFormatRules
        {
            get { return _completeRuleSet.Where(x => x.RuleType == RuleType.PreFormat).ToList(); }
            set { _completeRuleSet.AddRange(value);}
        }

        public RuleDefinition PrimaryRule
        {
            get { return _completeRuleSet.FirstOrDefault(x => x.RuleType == RuleType.Primary); }
            set
            {
                _completeRuleSet.RemoveAll(x => x.RuleType == RuleType.Primary);
                _completeRuleSet.Add(value);
            }
        }

        public List<RuleDefinition> DeDuplicationRules
        {
            get { return _completeRuleSet.Where(x => x.RuleType == RuleType.DeDuplication).ToList(); }
            set { _completeRuleSet.AddRange(value);}
        }

        public List<RuleDefinition> PostFormatRules
        {
            get { return _completeRuleSet.Where(x => x.RuleType == RuleType.PostFormat).ToList(); }
            set { _completeRuleSet.AddRange(value);}
        }

        //Constructors
        public RuleSet()
        {
            _completeRuleSet = new List<RuleDefinition>();
        }

        public RuleSet(string jsonObject)
        {
            _completeRuleSet = new List<RuleDefinition>();

            var tok = JObject.Parse(jsonObject);

            foreach (var i in tok)
            {
                if(i.Key == "PrimaryRule")
                    _completeRuleSet.Add(JsonConvert.DeserializeObject<RuleDefinition>(i.Value.ToString()));
                else
                    _completeRuleSet.AddRange(JsonConvert.DeserializeObject<List<RuleDefinition>>(i.Value.ToString()));
            }
            
        }

        public void AddRule(string ruleName, int ruleVer, RuleType ruleType)
        {
            if (ruleType == RuleType.Primary)
                _completeRuleSet.RemoveAll(x => x.RuleType == RuleType.Primary);

            _completeRuleSet.Add(new RuleDefinition
                                     {
                                         RuleName = ruleName,
                                         RuleVersion = ruleVer,
                                         RuleType = ruleType
                                     });

            
        }

        public List<RuleDefinition> GetCompleteRuleSet()
        {
            return _completeRuleSet;
        }
    }
}
