namespace _2GisTestTask
{
    public class UserType
    {
        public UserType(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override bool Equals(object? obj)
        {
            return obj is UserType userType &&
                   userType.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}