using System;
using System.Numerics;
using System.Threading;

class Program
{
    private static int _progress;
    private static bool _isCalculating = true;
    private static int _progressLine; // ilerleme çubuğu satırı kaydetme (save progress bar line)

    static void Main(string[] args)
    {
        Console.Write("Kaç basamak hesaplansın? ");
        int basamak = int.Parse(Console.ReadLine());

        // İlerleme çubuğu için boşluk bırak
        Console.WriteLine("\nİlerleme: 0%");
        _progressLine = Console.CursorTop - 1; // ilerleme satırını kaydet (save the progress bar)

        var progressThread = new Thread(ShowProgress);
        progressThread.Start();

        int ekstraHassasiyet = 5;
        BigInteger arctan5 = CalculateArcTan(5, basamak + ekstraHassasiyet, 0);
        BigInteger arctan239 = CalculateArcTan(239, basamak + ekstraHassasiyet, 50);

        BigInteger pi = 16 * arctan5 - 4 * arctan239;
        pi /= BigInteger.Pow(10, ekstraHassasiyet);

        _isCalculating = false;
        progressThread.Join(); // ilerleme thread'inin bitmesini bekle (wait for to progress thread to end)

        string piStr = pi.ToString().PadLeft(basamak + 1, '0');
        Console.WriteLine("\nπ = " + piStr.Insert(1, ".").Substring(0, basamak + 2));
    }

    private static void ShowProgress()
    {
        try
        {
            while (_isCalculating)
            {
                Console.SetCursorPosition(10, _progressLine);
                Console.Write($"{_progress}%  ");
                Thread.Sleep(50);
            }
            Console.SetCursorPosition(10, _progressLine);
            Console.Write("100%");
        }
        catch (ArgumentOutOfRangeException)
        {
            // konsol boyutu değişirse hata verme (don't error when scale of console changes)
        }
    }

    private static BigInteger CalculateArcTan(int x, int precision, int baseProgress)
    {
        BigInteger scale = BigInteger.Pow(10, precision);
        BigInteger result = 0;
        BigInteger term;
        bool isPositive = true;
        int k = 0;

        BigInteger xSquared = x * x;
        BigInteger power = scale / x;

        do
        {
            int denominator = 2 * k + 1;
            term = power / denominator;

            result += isPositive ? term : -term;
            isPositive = !isPositive;
            k++;
            power /= xSquared;

            // daha gerçekçi ilerleme hesabı için (for more realistic progress calculation)
            _progress = baseProgress + (int)((double)k / (precision / 0.8) * 50);
            _progress = Math.Min(_progress, baseProgress + 50);
        } while (term > 0);

        return result;
    }
}