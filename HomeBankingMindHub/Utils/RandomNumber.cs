using System;
using System.Security.Cryptography;

namespace HomeBankingMindHub.Utils
{
    public static class NumberGenerator
    {
        public static int GenerarNumero(long lowerBound, long upperBound)
        {
                Random random = new Random();
                int randomNumber = random.Next((int)lowerBound, (int)upperBound);
                return randomNumber;
        }
    }
    
}
