using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MusicStore
{
    class Security
    {
        //Podwójny sha256
        public static string DoubleHash(string s)
        {
            string hash = String.Empty;

            for (int i = 0; i < 2; i++)
            {
                // Inicjalizacja SHA256
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Obliczanie hasha
                    byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

                    // Konwersja bitow na string
                    foreach (byte b in hashValue)
                    {
                        hash += $"{b:X2}";
                    }
                }
            }

            return hash;
        }
        //Klasa od sprawdzania czy ktoś nie chce wrzucić do query niepożądanych znaków (sql injection)
        public static bool IsAlphanumeric(string s)
        {
            foreach(char c in s)
            {
                if (!Char.IsLetterOrDigit(c) && c != '_' && c != ' ')
                    return false;
            }
            return true;
        }
    }
}
