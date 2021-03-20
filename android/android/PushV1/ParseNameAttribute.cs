namespace PushV1
{
    #region using
    using System;
    #endregion

    [AttributeUsage(AttributeTargets.Class)]
    public class ParseNameAttribute : Attribute
    {
        public ParseNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}