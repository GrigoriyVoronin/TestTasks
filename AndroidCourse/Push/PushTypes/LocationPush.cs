using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class LocationPush : Push, ITime, ILocation
    {
        public float XCoard { get; set; }

        public float YCoard { get; set; }

        public int Radius { get; set; }

        public long ExpiryDate { get; set; }

        public LocationPush(string text, string type, float xCoard, float yCoard, int radius, long expiryDate) : base(text, type)
        {
            Radius = radius;
            XCoard = xCoard;
            YCoard = yCoard;
            ExpiryDate = expiryDate;
        }
    }
}
