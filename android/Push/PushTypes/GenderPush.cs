using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class GenderPush : Push, IGender
    {
        public string Gender { get;  set; }

        public GenderPush(string text, string type, string gender) : base(text,type)
        {
            Gender = gender;
        }
    }
}
