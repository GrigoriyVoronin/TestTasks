using System;
using System.Collections.Generic;

namespace Interest_clubs
{
    class Program
    {
        static void Main()
        {
            var (interestDevolopers, devoloperInterests) = InitializeData();
            var clubsCounter = CalculateClubsCount(interestDevolopers, devoloperInterests);
            Console.WriteLine(clubsCounter);
        }

        static int CalculateClubsCount(Dictionary<string, List<int>> interestDevolopers, Dictionary<int, string[]> devoloperInterests)
        {
            var clubsCounter = 0;
            var queue = new Queue<int>(devoloperInterests.Count);
            int indexToCheck;
            int devoloperIndex = 0;
            while (devoloperInterests.Count > 0)
            {
                if (queue.Count == 0)
                {
                    AddNewClub(devoloperInterests, ref devoloperIndex, queue);
                    clubsCounter++;
                }
                indexToCheck = queue.Dequeue();
                if (devoloperInterests.ContainsKey(indexToCheck))
                    CheckDevoloperInterests(devoloperInterests[indexToCheck], interestDevolopers, queue);
                devoloperInterests.Remove(indexToCheck);
            }
            return clubsCounter;
        }

        static void CheckDevoloperInterests(string[] devoloperInterests, Dictionary<string, List<int>> interestDevolopers, Queue<int> queue)
        {
            foreach (var interest in devoloperInterests)
            {
                if (interestDevolopers.ContainsKey(interest))
                {
                    foreach (var devoloper in interestDevolopers[interest])
                        queue.Enqueue(devoloper);
                    interestDevolopers.Remove(interest);
                }
            }
        }

        static void AddNewClub(Dictionary<int, string[]> devoloperInterests, ref int devoloperIndex, Queue<int> queue)
        {
            while (!devoloperInterests.TryGetValue(devoloperIndex, out _))
                devoloperIndex++;
            queue.Enqueue(devoloperIndex);
        }

        static (Dictionary<string, List<int>>, Dictionary<int, string[]>) InitializeData()
        {
            var countOfDevolopers = int.Parse(Console.ReadLine());
            var devoloperInterests = new Dictionary<int, string[]>(countOfDevolopers);
            var interestDevolopers = new Dictionary<string, List<int>>(countOfDevolopers);
            for (int i = 0; i < countOfDevolopers; i++)
            {
                devoloperInterests[i] = Console.ReadLine().Split();
                AddDevoloperInterests(devoloperInterests[i], interestDevolopers, i);
            }
            return (interestDevolopers, devoloperInterests);
        }

        static void AddDevoloperInterests(string[] devoloperInterests, Dictionary<string, List<int>> interestDevolopers, int i)
        {
            foreach (var interest in devoloperInterests)
            {
                if (!interestDevolopers.ContainsKey(interest))
                {
                    interestDevolopers[interest] = new List<int>();
                }
                interestDevolopers[interest].Add(i);
            }
        }
    }
}