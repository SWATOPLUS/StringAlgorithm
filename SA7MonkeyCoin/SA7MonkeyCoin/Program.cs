using System.IO;
using System.Numerics;

namespace SA7MonkeyCoin
{
    internal static class Program
    {
        private const string InputFileName = "coin.in";

        private const string OutputFileName = "coin.out";

        private static void Main()
        {
            var text = File.ReadAllText(InputFileName).Trim();
            var pi = BuildPrefixFunction(text);

            var result = BigInteger.Zero;

            for (var i = pi.Length; i > 0; i = pi[i - 1])
            {
                result += BigInteger.Pow(2, i);
            }

            File.WriteAllText(OutputFileName, result.ToString());
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
