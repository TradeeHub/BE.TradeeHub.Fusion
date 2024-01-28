namespace BE.TradeeHub.Fusion;

using Amazon;
public class AppSettings 
{
    public string Environment { get; set; }
    public string AppClientId { get; set; }
    public string UserPoolId { get; set; }
    public RegionEndpoint AWSRegion { get; set; }
    public string? AwsAccessKeyId { get; set; }
    public string? AwsSecretAccessKey { get; set; }
    public string[] AllowedDomains { get; set; }
    public string ValidIssuer { get; set; }
    
    public AppSettings(IConfiguration config)
    {
        Environment = config["ASPNETCORE_ENVIRONMENT"];
        AwsAccessKeyId = config["AWS_ACCESS_KEY_ID"];
        AwsSecretAccessKey = config["AWS_SECRET_ACCESS_KEY"];
        AllowedDomains = config.GetSection("AppSettings:AllowedOrigins").Get<string[]>();
        AppClientId = config.GetSection("AppSettings:Cognito:AppClientId").Value;
        UserPoolId = config.GetSection("AppSettings:Cognito:UserPoolId").Value;
        AWSRegion = RegionEndpoint.GetBySystemName(config.GetSection("AppSettings:Cognito:AWSRegion").Value);
        ValidIssuer = $"https://cognito-idp.{AWSRegion.SystemName}.amazonaws.com/{UserPoolId}";
        ValidateSettings();
    }
    
    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(AppClientId))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AppClientId");
        }

        if (string.IsNullOrEmpty(UserPoolId))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.UserPoolId");
        }
        
        if (string.IsNullOrEmpty(AwsAccessKeyId))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AwsAccessKeyId");
        }

        if (string.IsNullOrEmpty(AwsSecretAccessKey))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AwsSecretAccessKey");
        }
        
        if (AWSRegion == null)
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AWSRegion");
        }
        
        if (string.IsNullOrEmpty(ValidIssuer))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.ValidIssuer");
        }
        
        if (AllowedDomains.Length == 0)
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AllowedDomains");
        }
    }
}