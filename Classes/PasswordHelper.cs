using System;
using System.Security.Cryptography;

namespace School_Clinic_Patient_Record_System.Classes
{
    public static class PasswordHelper
    {
        private const int Iterations = 120000;
        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations))
                return "PBKDF2$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(derive.GetBytes(32));
        }
        public static bool Verify(string password, string encoded)
        {
            try
            {
                string[] parts = encoded.Split('$');
                if (parts.Length != 4 || parts[0] != "PBKDF2") return false;
                int rounds = Int32.Parse(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                using (var derive = new Rfc2898DeriveBytes(password, salt, rounds))
                {
                    byte[] actual = derive.GetBytes(expected.Length);
                    int diff = actual.Length ^ expected.Length;
                    for (int i = 0; i < Math.Min(actual.Length, expected.Length); i++) diff |= actual[i] ^ expected[i];
                    return diff == 0;
                }
            }
            catch { return false; }
        }
    }
}
