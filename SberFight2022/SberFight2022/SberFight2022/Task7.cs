using System;
using System.Collections.Generic;
using System.Linq;

namespace SberFight2022
{
    public class Task7
    {
        public static int GetResult(List<int> rocketPos, List<int> rocketSpeed)
        {
            
            var isUnioun = true;
            var oldDistances = new int[rocketPos.Count, rocketPos.Count];


            for (int i = 0; i < rocketPos.Count; i++)
            {
                for (int j = 0; j < rocketPos.Count; j++)
                {
                    oldDistances[i,j]= Math.Abs(rocketPos[i] - rocketPos[j]);
                }
            }

            while (isUnioun)
            {
                for (int i = 0; i < rocketPos.Count; i++)
                {
                    var pos = rocketPos[i];
                    var next = i + 1;
                    while (true)
                    {
                        var index = rocketPos.IndexOf(pos, next);
                        if (index != -1)
                        {
                            rocketPos.RemoveAt(index);
                            rocketSpeed[i] += rocketSpeed[index];
                            rocketSpeed.RemoveAt(index);
                            next = index + 1;
                            if (next >= rocketPos.Count)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                for (int i = 0; i < rocketPos.Count; i++)
                {
                    rocketPos[i]+=rocketSpeed[i];
                }

                var newDistances = new int[rocketPos.Count, rocketPos.Count];


                for (int i = 0; i < rocketPos.Count; i++)
                {
                    for (int j = 0; j < rocketPos.Count; j++)
                    {
                        newDistances[i, j] = Math.Abs(rocketPos[i] - rocketPos[j]);
                    }
                }

                var isOne = false;
                for (int i = 0; i < rocketPos.Count; i++)
                {
                    for (int j = 0; j < rocketPos.Count; j++)
                    {
                        if (newDistances[i, j] < oldDistances[i, j])
                        {
                            isOne = true;
                            break;
                        }
                    }

                    if (isOne)
                    {
                        break;
                    }
                }

                oldDistances = newDistances;

                isUnioun = isOne;
            }

            return rocketPos.Count;
        }

    }
}
/*
 * rocket_pos = [3, 11]
rocket_speed = [5, 1]
GetResult(rocket_pos, rocket_speed) = 1

rocket_pos = [2, 3]
rocket_speed = [1, 2]
GetResult(rocket_pos, rocket_speed) = 2 
*/