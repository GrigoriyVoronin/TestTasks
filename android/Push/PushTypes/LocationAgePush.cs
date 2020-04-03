using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    class LocationAgePush : Push, IAge, ILocation
    {
        public int Age { get;  set; }

        public float XCoard { get;  set; }

        public float YCoard { get; set; }

        public int Radius { get;  set; }

        public LocationAgePush(string text, string type, float xCoard, float yCoard, int radius,int age) : base(text, type)
        {
            Radius = radius;
            XCoard = xCoard;
            YCoard = yCoard;
            Age = age;
        }
    }
}
