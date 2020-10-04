using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SA57CyclicShift
{
    internal static class Program
    {
        private const string InputFileName = "input.txt";

        private const string OutputFileName = "output.txt";

        private const char Anchor = '#';

        private static void Main()
        {
            var lines = File.ReadAllLines(InputFileName);
            var result = new List<uint?>();

            for (var i = 0; i < lines.Length / 3; i++)
            {
                var source = lines[i * 3 + 1];
                var shifted = lines[i * 3 + 2];

                var shift = GetShift(source, shifted);

                result.Add(shift);
            }

            var output = string.Join("\n", result.Select(x => x?.ToString() ?? "-1"));

            File.WriteAllText(OutputFileName, output);
        }

        private static uint? GetShift(string source, string shifted)
        {
            return FindFirstSubstringIndex(source + source, shifted);
        }

        private static uint? FindFirstSubstringIndex(string source, string pattern)
        {
            var prefixFunction = BuildPrefixFunction(pattern + Anchor + source);

            for (var i = (uint)pattern.Length + 1; i < prefixFunction.Length; i++)
            {
                if (prefixFunction[i] == pattern.Length)
                {
                    return i - (uint)pattern.Length - (uint)pattern.Length;
                }
            }

            return null;
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
