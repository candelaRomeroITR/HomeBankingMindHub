using System;
using System.Security.Cryptography;

namespace HomeBankingMindHub.Utils
{
    public static class NumberGenerator
    {
        public static int GenerarNumero(int lowerBound, int upperBound)
        {
                Random random = new Random();
                int randomNumber = random.Next(lowerBound, upperBound);
                return randomNumber;
        }
    }
    
}
