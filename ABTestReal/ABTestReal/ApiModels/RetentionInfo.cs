using System;
using System.Collections.Generic;

namespace ABTestReal.ApiModels
{
    public class RetentionInfo
    {
        public double RollingRetention { get; set; }
        public List<UserLifeSpan> UserLifeSpans { get; set; }
    }
}