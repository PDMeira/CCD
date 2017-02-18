using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MergeEngine;

namespace UI
{
    public static class DefaultRuleset
    {
        public static RuleSet GetDefaultRuleset()
        {
            var retVal = new RuleSet();

            retVal.AddRule("StripText", 0, RuleType.PreFormat);
            retVal.AddRule("PrimaryMergeRuleWithValidation", 0, RuleType.Primary);
            retVal.AddRule("Author", 0, RuleType.DeDuplication);
            retVal.AddRule("Confidentiality", 0, RuleType.DeDuplication);
            retVal.AddRule("MedicationRule1", 0, RuleType.DeDuplication);
            retVal.AddRule("SocialHistory_EtohUse", 0, RuleType.DeDuplication);
            retVal.AddRule("SocialHistory_SmokingConsolidation", 0, RuleType.DeDuplication);
            retVal.AddRule("TestProblemsDedup", 0, RuleType.DeDuplication);
            retVal.AddRule("AddFormatedText", 0, RuleType.PostFormat);

            return retVal;

        }
    }
}
