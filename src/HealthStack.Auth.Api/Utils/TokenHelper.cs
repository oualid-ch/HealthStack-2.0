namespace HealthStack.Auth.Api.Utils
{
    public static class TokenHelper
    {
        public static string? ExtractTokenFromHeader(HttpContext http)
        {
            if (!http.Request.Headers.TryGetValue("Authorization", out var raw)) return null;

            var value = raw.ToString();

            // Expected header: "Bearer fake-token-<UserId>"
            if (!value.StartsWith("Bearer ")) return null;

            return value.Replace("Bearer ", "");
        }
    }
}
