using System.Text.Json.Serialization;

namespace Excalibur.Authentication;

public class Token
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
