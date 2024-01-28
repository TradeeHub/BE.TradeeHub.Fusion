using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace BE.TradeeHub.Fusion;

public class AuthService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly AppSettings _appSettings;

    public AuthService(IAmazonCognitoIdentityProvider cognitoService, AppSettings appSettings)
    {
        _cognitoService = cognitoService;
        _appSettings = appSettings;
    }

    public async Task<InitiateAuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken, string? deviceKey = null)
    {
        try
        {
            var tokenRequest = new InitiateAuthRequest
            {
                ClientId = _appSettings.AppClientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            if (!string.IsNullOrEmpty(deviceKey))
            {
                tokenRequest.AuthParameters.Add("DEVICE_KEY", deviceKey);
            }

            return await _cognitoService.InitiateAuthAsync(tokenRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine(ex.Message);
            throw; // Rethrow the exception to handle it outside this method
        }
    }
}