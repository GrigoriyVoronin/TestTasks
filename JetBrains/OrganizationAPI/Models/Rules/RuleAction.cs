namespace OrganizationApi.Models.Rules
{
    public readonly struct RuleAction
    {
        private readonly int _addStamp;
        private readonly int _removeStamp;
        private readonly int _moveDepartmentNumber;

        public static implicit operator RuleAction((int addStamp, int removeStamp, int moveDepartment) parameters)
        {
            var (addStamp, removeStamp, moveDepartment) = parameters;
            return new RuleAction(addStamp, removeStamp, moveDepartment);
        }

        public RuleAction(int addStamp, int removeStamp, int moveDepartmentNumber)
        {
            _addStamp = addStamp;
            _removeStamp = removeStamp;
            _moveDepartmentNumber = moveDepartmentNumber;
        }

        internal void Execute(BypassSheet bypassSheet)
        {
            bypassSheet.AddStamp(_addStamp);
            bypassSheet.RemoveStamp(_removeStamp);
            bypassSheet.MoveTo(_moveDepartmentNumber);
        }
    }
}