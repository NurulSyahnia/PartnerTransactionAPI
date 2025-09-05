using System.Security.Cryptography;
using System.Text;

namespace PartnerTransactionAPI.Utilities
{
    public class SignatureHelper
    {
        public static string GenerateSignature(string timestamp, string partnerKey, string partnerRefNo, long totalAmount, string partnerPassword)
        {
            // Concatenate parameters
            string rawMessage = timestamp + partnerKey + partnerRefNo + totalAmount + partnerPassword;

            // Apply SHA-256 hashing (UTF-8 input)
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawMessage));

                // Hexadecimal lowercase string
                string hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // Convert hex string to Base64
                string base64Hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(hexHash));

                return base64Hash;
            }
        }
    }
}
