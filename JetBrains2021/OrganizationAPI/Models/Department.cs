using OrganizationApi.Models.Rules;

namespace OrganizationApi.Models
{
    public class Department
    {
        internal Department(int number, IDepartmentRule rule)
        {
            Number = number;
            Rule = rule;
        }

        internal int Number { get; }
        internal IDepartmentRule Rule { get; }

        public static Department WithConditionRule(int number, ConditionRule conditionRule)
        {
            return new Department(number, conditionRule);
        }

        public static Department WithSimpleRule(int number, SimpleRule simpleRule)
        {
            return new Department(number, simpleRule);
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Department department &&
                   department.Number == Number;
        }
    }
}