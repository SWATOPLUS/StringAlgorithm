using System;
using System.Linq;

namespace SA0SuffixArray
{
    internal static class Program
    {
        private static void Main()
        {
            var line = Console.ReadLine()?.Trim() ?? throw new InvalidOperationException();
            var suffixArray = BuildSuffixArray(line);

            Console.WriteLine(suffixArray.Length);
            Console.WriteLine(string.Join(" ", suffixArray.Select(x => x + 1)));
        }

        private static int[] BuildSuffixArray(string str)
        {
            var n = str.Length;
            var order = Enumerable.Range(0, n)
                .Select(x => n - 1 - x)
                .ToArray();

            for (var i = 0; i < n; i++)
            {
                order[i] = n - 1 - i;
            }

            order = order
                .OrderBy(x => str[x])
                .ToArray();

            var suffixArray = new int[n];
            var classes = new int[n];

            for (var i = 0; i < n; i++)
            {
                suffixArray[i] = order[i];
                classes[i] = str[i];
            }

            for (var shift = 1; shift < n; shift *= 2)
            {
                var oldClasses = classes.ToArray();

                for (var i = 0; i < n; i++)
                {
                    if (i > 0
                        && oldClasses[suffixArray[i - 1]] == oldClasses[suffixArray[i]]
                        && suffixArray[i - 1] + shift < n
                        && oldClasses[suffixArray[i - 1] + shift / 2] == oldClasses[suffixArray[i] + shift / 2])
                    {
                        classes[suffixArray[i]] = classes[suffixArray[i - 1]];
                    }
                    else
                    {
                        classes[suffixArray[i]] = i;
                    }
                }

                var counters = Enumerable.Range(0, n).ToArray();
                var oldSuffixArray = suffixArray.ToArray();

                for (var i = 0; i < n; i++)
                {
                    var s1 = oldSuffixArray[i] - shift;

                    if (s1 >= 0)
                    {
                        suffixArray[counters[classes[s1]]++] = s1;
                    }
                }
            }
            return suffixArray;
        }
    }
}
