using System.Net;

namespace BE.TradeeHub.Fusion
{
    public class CookiePropagatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthService _authService;

        public CookiePropagatingHandler(IHttpContextAccessor httpContextAccessor, AuthService authService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var cookieHeader = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            
            cookieHeader = await RefreshJwtIfEmptyAndThereIsRefreshToken(cancellationToken, cookieHeader);

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                request.Headers.Add("Cookie", cookieHeader);
            }
            
            var response = await base.SendAsync(request, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!responseContent.Contains("AUTH_NOT_AUTHORIZED")) return SetCookiesIfAnyAndReturnResponse(response);
            
            cookieHeader = await RefreshJwtIfEmptyAndThereIsRefreshToken(cancellationToken, cookieHeader);

            if (string.IsNullOrEmpty(cookieHeader)) return SetCookiesIfAnyAndReturnResponse(response);
            request.Headers.Remove("Cookie");
            request.Headers.Add("Cookie", cookieHeader);
            var newResponse = await base.SendAsync(request, cancellationToken);
            return SetCookiesIfAnyAndReturnResponse(newResponse);
        }

        private HttpResponseMessage SetCookiesIfAnyAndReturnResponse(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues("Set-Cookie", out var cookies)) return response;
            
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return response;
            
            foreach (var cookie in cookies)
            {
                httpContext.Response.Headers.Append("Set-Cookie", cookie);
            }

            return response;
        }

        private async Task<string?> RefreshJwtIfEmptyAndThereIsRefreshToken(CancellationToken cancellationToken, string? cookieHeader)
        {
            var jwt = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(jwt)) return cookieHeader;
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken)) return cookieHeader;
            var authResponse = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
            
            if (authResponse.HttpStatusCode != HttpStatusCode.OK) return cookieHeader;
            var newJwtToken = authResponse.AuthenticationResult.IdToken;
            var jwtExpiresIn = authResponse.AuthenticationResult.ExpiresIn;
            var jwtExpirationTime = DateTime.UtcNow.AddSeconds(jwtExpiresIn);

            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Ensure this is true for HTTPS
                SameSite = SameSiteMode.Strict, // Or None if necessary
                Expires = jwtExpirationTime,
                Path = "/"
            };
            
            cookieHeader = $"{cookieHeader}; jwt={newJwtToken}";
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("jwt", newJwtToken, jwtCookieOptions);

            return cookieHeader;
        }
    }
}
