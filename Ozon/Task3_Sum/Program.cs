using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Task3_Sum
{
    internal class Program
    {
        private static void Main()
        {
            var result = 0;
            var t = true;
            var diffs = new HashSet<int>();
            int dif;
            var simbols = new char[16];
            var len = 0;
            int numb;
            using (var a = new StreamReader("input.txt"))
            {
                while (!a.EndOfStream)
                {
                    a.Read(simbols, len, 1);
                    if (simbols[len] == 10 || simbols[len] == 13 || simbols[len] == 32)
                    {
                        if (t)
                        {
                            result = int.Parse(new string(simbols, 0,len));
                            t = false;
                            len = 0;
                            continue;
                        }

                        if (len > 0)
                        {
                            numb = int.Parse(new string(simbols, 0,len));
                            len = 0;
                            if (diffs.Contains(numb))
                            {
                                End(1);
                                return;
                            }

                            dif = result - numb;
                            if (dif > 0)
                                diffs.Add(dif);
                        }

                        continue;
                    }

                    len++;
                }

                if (len > 0)
                {
                    numb = int.Parse(new string(simbols, 0,len));
                    if (diffs.Contains(numb))
                    {
                        End(1);
                        return;
                    }

                    dif = result - numb;
                    if (dif > 0)
                        diffs.Add(dif);
                }
            }

            End(0);
        }


        private static void End(int answer)
        {
            File.WriteAllText("output.txt", answer.ToString());
        }
    }
}