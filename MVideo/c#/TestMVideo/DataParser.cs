#region using

using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestMVideo.Models;

#endregion

namespace TestMVideo
{
    public class DataParser
    {
        public IEnumerable<InputLine> ParseCsvFile(string filePath)
        {
            return File.ReadLines(filePath)
                .Select(csvLine => new InputLine(csvLine));
        }
    }
}