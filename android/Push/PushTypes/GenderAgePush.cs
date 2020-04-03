using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    class GenderAgePush : Push, IAge, IGender
    {
        public string Gender { get; set; }

        public int Age { get; set; }

        public GenderAgePush (string text, string type, int age, string gender) : base(text, type)
        {
            Age = age;
            Gender = gender;
        }
    }
}
