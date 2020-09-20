using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Utils
{
    public static class AspCoreExtensions
    {
        public static string HashPassword(this string password, ref string saltText)
        {
            var salt = new byte[128 / 8];

            if (saltText != null)
            {
                salt = Convert.FromBase64String(saltText);
            }
            else
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);

                    saltText = Convert.ToBase64String(salt);
                }
            }

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
        }
    }
}
