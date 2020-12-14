using System;
using System.IO;
using System.Linq;

namespace SA62ThreeOccurrences
{
    internal static class Program
    {
        private const string InputFileName = "input.txt";

        private const string OutputFileName = "output.txt";

        private static void Main()
        {
            var lines = File.ReadAllLines(InputFileName);
            var results = lines.Select(FindThreeOccurrences);
            var output = string.Join("\n", results);

            File.WriteAllText(OutputFileName, output);
        }

        private static string FindThreeOccurrences(string s)
        {
            var zFunctions = new int[s.Length][];

            for (var i = 0; i < zFunctions.Length; i++)
            {
                zFunctions[i] = CalcZFunction(s.AsSpan(i));

                for (var j = 0; j < zFunctions[i].Length; j++)
                {
                    zFunctions[i][j] = Math.Min(zFunctions[i][j], j);
                }
            }

            var zFunctionMaxes = zFunctions
                .Select(x => x.Max())
                .ToArray();

            var resultLength = 0;
            var resultPosition = 0;

            for (var i = 0; i < zFunctions.Length; i++)
            {
                for (var j = 1; j < zFunctions[i].Length; j++)
                {
                    var commonPrefixLength = Math.Min(zFunctions[i][j], zFunctionMaxes[i + j]);

                    if (commonPrefixLength > resultLength)
                    {
                        resultLength = commonPrefixLength;
                        resultPosition = i;
                    }
                }
            }

            return s.Substring(resultPosition, resultLength);
        }

        private static int[] CalcZFunction(ReadOnlySpan<char> chars)
        {
            var result = new int[chars.Length];

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
