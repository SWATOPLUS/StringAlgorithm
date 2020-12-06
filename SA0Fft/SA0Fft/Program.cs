using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SA0Fft
{
    internal static class Program
    {
        private static void Main()
        {
            Console.ReadLine();
            var lineA = Console.ReadLine() ?? throw new InvalidOperationException();

            Console.ReadLine();
            var lineB = Console.ReadLine() ?? throw new InvalidOperationException();

            var polyA = lineA.Split().Select(int.Parse).Select(x => new Complex(x, 0)).ToArray();
            var polyB = lineB.Split().Select(int.Parse).Select(x => new Complex(x, 0)).ToArray();

            var result = PolyMul(polyA, polyB)
                .Select(x => (int)Math.Round(x))
                .TrimEndBy(x => x == 0)
                .ToArray();

            Console.WriteLine(result.Length);
            Console.WriteLine(string.Join(" ", result));
        }

        private static IEnumerable<T> TrimEndBy<T>(this IEnumerable<T> source, Func<T, bool> trimPredicate)
        {
            var array = source.ToArray();
            var size = array.Length;

            while (size > 0 && trimPredicate.Invoke(array[size - 1]))
            {
                size--;
            }

            return array.Take(size);
        }

        private static double[] PolyMul(Complex[] a, Complex[] b)
        {
            var maxLength = a.Length + b.Length;
            var length = (int)Math.Pow(2, Math.Ceiling(Math.Log(maxLength, 2)));

            var aEx = a.Concat(Zeros().Take(length - a.Length)).ToArray();
            var bEx = b.Concat(Zeros().Take(length - b.Length)).ToArray();

            var aF = Fft(aEx);
            var bF = Fft(bEx);

            return FftInverse(CoordinateMul(aF, bF));
        }

        private static IEnumerable<Complex> Zeros()
        {
            while (true)
            {
                yield return Complex.Zero;
            }
        }

        private static Complex[] CoordinateMul(Complex[] a, Complex[] b)
        {
            var result = new Complex[Math.Max(a.Length, b.Length)];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = a.ElementAtOrDefault(i) * b.ElementAtOrDefault(i);
            }

            return result;
        }

        private static Complex Rotate(int k, int N)
        {
            if (k % N == 0)
            {
                return 1;
            }

            var arg = -2 * Math.PI * k / N;

            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        private static Complex[] Fft(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = Fft(x_even);
                Complex[] X_odd = Fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + Rotate(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - Rotate(i, N) * X_odd[i];
                }
            }
            return X;
        }

        private static double[] FftInverse(Complex[] X)
        {
            var a = X
                .Select(Complex.Conjugate)
                .ToArray();

            return Fft(a)
                .Select(Complex.Conjugate)
                .Select(x => x / X.Length)
                .Select(x => x.Real)
                .ToArray();
        }
    }
}