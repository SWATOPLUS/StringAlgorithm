using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SA52Period
{
    internal static class Program
    {
        private const string InputFileName = "input.txt";

        private const string OutputFileName = "output.txt";

        private static void Main()
        {
            var lines = File.ReadAllLines(InputFileName);

            var prefFunction = BuildPrefixFunction(lines[1]);
            var result = new List<(int PrefixSize, int PatternCount)>();

            for (var i = 0; i < prefFunction.Length; i++)
            {
                var prefixFunctionValue = prefFunction[i];
                var prefixSize = i + 1;
                var patternSize = prefixSize - prefixFunctionValue;

                if (prefixFunctionValue != 0 && prefixSize % patternSize == 0)
                {
                    result.Add((prefixSize, prefixSize / patternSize));
                }
            }

            var output = string.Join("\n", result.Select(x => $"{x.PrefixSize} {x.PatternCount}"));

            File.WriteAllText(OutputFileName, output);
        }

        private static int[] BuildPrefixFunction(string s)
        {
            var result = new int[s.Length];

            for (var i = 1; i < s.Length; i++)
            {
                var j = result[i - 1];

                while (j > 0 && s[i] != s[j])
                {
                    j = result[j - 1];
                }

                if (s[i] == s[j])
                {
                    j++;
                }

                result[i] = j;
            }

            return result;
        }
    }
}
