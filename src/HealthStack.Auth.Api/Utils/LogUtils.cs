namespace HealthStack.Auth.Api.Utils
{
    public class LogUtils()
    {
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return "****";

            var parts = email.Split('@');
            if (parts.Length != 2) return "****";

            var name = parts[0];
            var domain = parts[1];

            // Show first character of local part + ****
            string maskedName = name.Length > 1 ? $"{name[0]}****" : "****";

            // Show first character of domain + ****
            string domainName = domain.Length > 1 ? $"{domain[0]}****" : "****";

            return $"{maskedName}@{domainName}";
        }

        public static string MaskGuid(Guid id)
        {
            // Show only first 4 chars + ****
            var str = id.ToString();
            return $"{str[..4]}****";
        }
    }
}