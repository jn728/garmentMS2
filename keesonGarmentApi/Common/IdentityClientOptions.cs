namespace keesonGarmentApi.Common;

public class IdentityClientOptions
{
    public string Authority { get; set; }
    public bool UseHttps { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public string Scope { get; set; }
    public string ApiName { get; set; }
    public string ApiSecret { get; set; }
    public bool SaveToken { get; set; }
}