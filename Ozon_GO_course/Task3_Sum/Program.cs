using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Task3_Sum
{
    internal class Program
    {
        private static void Main()
        {
            var numbers = ReadFile1();
            var t = TakeTarget();
            var start = 1;
            var end = numbers.Count - 2;
            if (end < 1)
            {
                End(0);
                return;
            }

            numbers.Sort(1, numbers.Count - 2, null);
            int currentDiffrent;
            int oldStart;
            while (true)
            {
                oldStart = start;
                currentDiffrent = t.Item1 - numbers[start];
                while (numbers[end] > currentDiffrent && end > start)
                    end--;
                if (end <= start)
                    break;

                currentDiffrent = t.Item1 - numbers[end];
                while (numbers[start] < currentDiffrent && start < end)
                    start++;
                if (oldStart == start)
                {
                    End(1);
                    return;
                }

                if (end <= start)
                    break;
            }

            End(0);
        }


        private static void End(int answer)
        {
            File.WriteAllText("output.txt", answer.ToString());
        }

        private static Tuple<int, int> TakeTarget()
        {
            var f = File.OpenRead("input.txt");
            var buf = new byte[1];
            var position = 0;
            byte[] data;
            while (true)
            {
                f.Read(buf, 0, 1);
                if (buf[0] == 13)
                {
                    data = new byte[position];
                    f.Position = 0;
                    f.Read(data, 0, position);
                    position += 2;
                    return new Tuple<int, int>(int.Parse(Encoding.UTF8.GetString(data)), position);
                }

                position++;
            }
        }

        private static List<int> ReadFile1()
        {
            var a = File.OpenRead("input.txt");
            var answer = new List<int>();
            var str = new StringBuilder(10);
            var buf = new byte[1];
            string ch;
            while (a.Position<a.Length)
            {
                while (a.Position<a.Length)
                {
                    a.Read(buf, 0, 1);
                    ch = Encoding.UTF8.GetString(buf);
                    if (ch == " " || ch == "\n" || ch == "\r")
                        break;

                    str.Append(ch);
                }

                if(str.Length>0)
                    answer.Add(int.Parse(str.ToString()));
                str.Clear();
            }

            return answer;
        }
    }
}