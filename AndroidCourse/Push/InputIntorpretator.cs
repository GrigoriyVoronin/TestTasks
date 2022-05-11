using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    class InputIntorpretator
    {
        public string Type { get; private set; }

        public string Text { get; private set; }

        public string Gender { get; private set; }

        public int Age { get; private set; }

        public int OsVersion { get; private set; }

        public int Radius { get; private set; }

        public float XCoard { get; private set; }

        public float YCoard { get; private set; }

        public long ExpiryDate { get; private set; }

        public long Time { get; private set; }

        public InputIntorpretator(int lineCount)
        {
            for(int i=0; i<lineCount;i++)
            {
                var str = Console.ReadLine().Split();
                switch (str[0])
                {
                    case "text":
                        Text = str[1];
                        break;
                    case "type":
                        Type = str[1];
                        break;
                    case "x_coord":
                        XCoard = float.Parse(str[1], System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "y_coord":
                        YCoard = float.Parse(str[1], System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "radius":
                        Radius = int.Parse(str[1]);
                        break;
                    case "expiry_date":
                        ExpiryDate = long.Parse(str[1]);
                        break;
                    case "gender":
                        Gender = str[1];
                        break;
                    case "age":
                        Age = int.Parse(str[1]);
                        break;
                    case "time":
                        Time = long.Parse(str[1]);
                        break;
                    case "os_version":
                        OsVersion = int.Parse(str[1]);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
