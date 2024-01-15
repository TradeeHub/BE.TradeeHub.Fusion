namespace BE.TradeeHub.Fusion;

public class CookiePropagatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookiePropagatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookieHeader = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
        
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.Add("Cookie", cookieHeader);
        }
        
        var response = await base.SendAsync(request, cancellationToken);

        // Check if the response contains the 'Set-Cookie' header
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies)) return response;
        
        // Add the 'Set-Cookie' header to the outgoing response
        
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return response;
        
        foreach (var cookie in cookies)
        {
            httpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return response;
    }
}