using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace AnimalFarm.Utils.Security
{
    /// <summary>
    /// Encapsulates logic for hashing user passwords.
    /// </summary>
    public class PasswordHasher
    {
        public string GetHash(string salt, string password)
        {
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
