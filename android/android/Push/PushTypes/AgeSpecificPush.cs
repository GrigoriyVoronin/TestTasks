using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class AgeSpecificPush : Push, IAge, ITime
    {
        public int Age { get; set; }

        public long ExpiryDate { get; set; }

        public AgeSpecificPush(string text, string type, int age, long expiryDate) : base(text, type)
        {
            Age = age;
            ExpiryDate = expiryDate;
        }
    }
}
