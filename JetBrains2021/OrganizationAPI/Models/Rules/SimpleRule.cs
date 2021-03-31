namespace OrganizationApi.Models.Rules
{
    public class SimpleRule : IDepartmentRule
    {
        private readonly RuleAction _action;

        public SimpleRule(RuleAction action)
        {
            _action = action;
        }

        void IDepartmentRule.Execute(BypassSheet bypassSheet)
        {
            _action.Execute(bypassSheet);
        }

        public static implicit operator SimpleRule((int addStamp, int removeStamp, int moveToDepartment) rule)
        {
            return new SimpleRule(rule);
        }
    }
}