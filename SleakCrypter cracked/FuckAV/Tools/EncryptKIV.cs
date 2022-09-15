using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FuckAV.Tools
{
    internal class EncryptKIV
    {
        private static byte[] GetIV(int num)
        {
            var randomBytes = new byte[num];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        private static byte[] GetKey(int size)
        {

            byte[] CKey = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(CKey);
            }
            return CKey;
        }
        public static string RandString(int size)
        {
            Random rand = new Random();

            String str = "abcdefghijklmnopqrstuvwxyz0123456789";

            // Initializing the empty string
            String randomstring = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly
                int x = rand.Next(str.Length);

                // Appending the character at the 
                // index to the random alphanumeric string.
                randomstring = randomstring + str[x];
            }
            return randomstring;
        }
        public static string GetAesKey()
        {
            string randString = RandString(16);

            byte[] RandomKey = GetKey(randString.Length);

            string Akey = Convert.ToBase64String(RandomKey);

            return Akey;
        }
        public static string GetAesIV()
        {
            string randString = RandString(16);

            byte[] RandomKey = GetIV(randString.Length);

            string AIV = Convert.ToBase64String(RandomKey);

            return AIV;
        }
        public static string GetTrippleKey()
        {
            string randString = RandString(16);

            byte[] RandomKey = GetKey(randString.Length);

            string Akey = Convert.ToBase64String(RandomKey);

            return Akey;
        }
        public static string GetTrippleIV()
        {
            string randString = RandString(8);

            byte[] RandomKey = GetIV(randString.Length);

            string AIV = Convert.ToBase64String(RandomKey);

            return AIV;
        }
    }
}
