using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SA61UniqueSubstringCount
{
    internal static class Program
    {
        private const string InputFileName = "input.txt";

        private const string OutputFileName = "output.txt";

        private static void Main()
        {
            var lines = File.ReadAllLines(InputFileName);
            var results = lines.Select(CountUniqueSubstrings);
            var output = string.Join("\n", results);

            File.WriteAllText(OutputFileName, output);
        }

        private static int CountUniqueSubstrings(string s)
        {
            return Enumerable.Range(1, s.Length)
                .Select(x => s.Take(x).Reverse().ToArray())
                .Select(CalcZFunction)
                .Select(x => x.Length - x.Max())
                .Sum();
        }

        private static int[] CalcZFunction(IReadOnlyList<char> chars)
        {
            var result = new int[chars.Count];

            var i = 1;
            var l = 0;
            var r = 0;

            while (i < result.Length)
            {
                if (i <= r)
                {
                    result[i] = Math.Min(r - i + 1, result[i - l]);
                }

                while (i + result[i] < result.Length && chars[result[i]] == chars[i + result[i]])
                {
                    result[i]++;
                }

                if (i + result[i] - 1 > r)
                {
                    l = i;
                    r = i + result[i] - 1;
                }

                i++;
            }

            return result;
        }
    }
}
