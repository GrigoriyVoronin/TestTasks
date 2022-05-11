namespace OrganizationApi.Models.Rules
{
    public class ConditionRule : IDepartmentRule
    {
        private readonly int _containingStamp;
        private readonly RuleAction _failureAction;
        private readonly RuleAction _successAction;

        public ConditionRule(int containingStamp, RuleAction successAction, RuleAction failureAction)
        {
            _containingStamp = containingStamp;
            _successAction = successAction;
            _failureAction = failureAction;
        }

        void IDepartmentRule.Execute(BypassSheet bypassSheet)
        {
            if (bypassSheet.Contains(_containingStamp))
                _successAction.Execute(bypassSheet);
            else
                _failureAction.Execute(bypassSheet);
        }
    }
}