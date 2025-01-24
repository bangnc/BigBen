using System.Text;

namespace BangKaAPI
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
                !IsValidAuthHeader(authHeader))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            await _next(context);
        }

        private bool IsValidAuthHeader(string authHeader)
        {
            var encodedCredentials = authHeader.Split(' ')[1];
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var parts = decodedCredentials.Split(':');
            var username = parts[0];
            var password = parts[1];

            // Replace with your own validation logic
            return username == "admin" && password == "password";
        }
    }

}
