using System.Security.Cryptography;
using System.Text;

namespace PwnedPasswords
{
    internal static class SHA1Util
    {
        private static readonly SHA1 _sha1 = SHA1.Create();

        public static string SHA1HashStringForUTF8String(string s)
        {
            byte[] bytes = Encoding.Default.GetBytes(s);

            byte[] hashBytes = _sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("X2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
