using System;
using System.Collections.Generic;

namespace Interest_clubs
{
    internal class Program
    {
        private static void Main()
        {
            var (interestDevelopers, developerInterests) = ParseInput();
            var clubsCounter = CalculateClubsCount(interestDevelopers, developerInterests);
            Console.WriteLine(clubsCounter);
        }

        private static int CalculateClubsCount(IDictionary<string, List<int>> interestDevelopers,
            Dictionary<int, string[]> developerInterests)
        {
            var clubsCounter = 0;
            var developersToCheck = new Queue<int>(developerInterests.Count);
            var developerIndex = 0;

            while (developerInterests.Count > 0)
            {
                if (developersToCheck.Count == 0)
                {
                    AddNewClub(developerInterests, ref developerIndex, developersToCheck);
                    clubsCounter++;
                }

                var indexToCheck = developersToCheck.Dequeue();
                if (developerInterests.ContainsKey(indexToCheck))
                    CheckDeveloperInterests(developerInterests[indexToCheck], interestDevelopers, developersToCheck);
                developerInterests.Remove(indexToCheck);
            }

            return clubsCounter;
        }

        private static void CheckDeveloperInterests(IEnumerable<string> developerInterests,
            IDictionary<string, List<int>> interestDevelopers, Queue<int> developersToCheck)
        {
            foreach (var interest in developerInterests)
            {
                if (!interestDevelopers.ContainsKey(interest))
                    continue;
                foreach (var developer in interestDevelopers[interest])
                    developersToCheck.Enqueue(developer);
                interestDevelopers.Remove(interest);
            }
        }

        private static void AddNewClub(IReadOnlyDictionary<int, string[]> developerInterests, ref int developerIndex,
            Queue<int> developersToCheck)
        {
            while (!developerInterests.TryGetValue(developerIndex, out _))
                developerIndex++;
            developersToCheck.Enqueue(developerIndex);
        }

        private static (Dictionary<string, List<int>>, Dictionary<int, string[]>) ParseInput()
        {
            var countOfDevelopers = int.Parse(Console.ReadLine());
            var developerInterests = new Dictionary<int, string[]>(countOfDevelopers);
            var interestDevelopers = new Dictionary<string, List<int>>(countOfDevelopers);
            for (var i = 0; i < countOfDevelopers; i++)
            {
                developerInterests[i] = Console.ReadLine().Split();
                AddDeveloperInterests(developerInterests[i], interestDevelopers, i);
            }

            return (interestDevelopers, developerInterests);
        }

        private static void AddDeveloperInterests(IEnumerable<string> developerInterests,
            IDictionary<string, List<int>> interestDevelopers, int i)
        {
            foreach (var interest in developerInterests)
            {
                if (!interestDevelopers.ContainsKey(interest))
                    interestDevelopers[interest] = new List<int>();
                interestDevelopers[interest].Add(i);
            }
        }
    }
}