using System.Collections.Generic;
using System;

namespace GitTask
{
    public class Git
    {
        public ushort CommitNumber;
        public int [] GitFiles;
        public Dictionary <int, Dictionary <int, int>> CommitedGits;

        public Git(int filesCount)
        {
            GitFiles = new int [filesCount];
            CommitedGits = new Dictionary <int, Dictionary<int, int>> ();
            CommitedGits [0] = new Dictionary <int, int> ();
        }

        public void Update(int fileNumber, int value)
        {
            CommitedGits [CommitNumber] [fileNumber] = value;
        }

        public int Commit()
        {
            CommitNumber++;
            CommitedGits [CommitNumber] = new Dictionary <int, int> ();
            return CommitNumber-1;
        }

        public int Checkout(int commitNumber, int fileNumber)
        {
            if (commitNumber >= CommitNumber)
                throw new ArgumentException ();
            else
                return CalculateValue (commitNumber, fileNumber);
        }

        public int CalculateValue (int commitNumber, int fileNumber)
        {
            for (int i = commitNumber; i >= 0; i--)
            {
                if (CommitedGits [i].ContainsKey(fileNumber))
                    return CommitedGits [i] [fileNumber];
                else
                    continue;
            }
            return GitFiles [fileNumber];
        }
    }
}