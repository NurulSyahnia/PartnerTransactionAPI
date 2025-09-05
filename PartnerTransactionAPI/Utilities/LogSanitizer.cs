using System.Text.Json;

namespace PartnerTransactionAPI.Utilities
{
    public class LogSanitizer
    {
        private static readonly string[] SensitiveFields =
        {
            "password", "partnerpassword", "secret", "token"
        };

        public static string Sanitize<T>(T obj)
        {
            if (obj == null) return string.Empty;

            var json = JsonSerializer.Serialize(obj);

            foreach (var field in SensitiveFields)
            {
                json = System.Text.RegularExpressions.Regex.Replace(
                    json,
                    $"(\"{field}\":\\s*\")(.*?)(\")",
                    $"$1***MASKED***$3",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }

            return json;
        }
    }
}
