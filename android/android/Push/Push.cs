using System;


namespace Push
{
    abstract class Push
    {
        public string Text { get; protected set; }

        public string PushType { get; protected set; }

        protected Push(string text, string type)
        {
            Text = text;
            PushType = type;
        }

        public static Push CreateNewPush(string type, InputParser input)
        {
            switch (type)
            {
                case "LocationPush":
                    return new LocationPush(input.Text, input.Type, input.XCoard, input.YCoard, input.Radius, input.ExpiryDate);
                case "AgeSpecificPush":
                    return new AgeSpecificPush(input.Text, input.Type, input.Age, input.ExpiryDate);
                case "TechPush":
                    return new TechPush(input.Text, input.Type, input.OsVersion);
                case "LocationAgePush":
                    return new LocationAgePush(input.Text, input.Type, input.XCoard, input.YCoard, input.Radius, input.Age);
                case "GenderAgePush":
                    return new GenderAgePush(input.Text, input.Type, input.Age, input.Gender);
                case "GenderPush":
                    return new GenderPush(input.Text, input.Type, input.Gender);
                default:
                    throw new Exception("Unknown type of push");
            };
        }
    }
}
