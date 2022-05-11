namespace OrganizationApi.Models.Rules
{
    public interface IDepartmentRule
    {
        internal void Execute(BypassSheet bypassSheet);
    }
}