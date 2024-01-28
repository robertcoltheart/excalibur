using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Excalibur.Configuration;

namespace Excalibur.Authentication;

public class PkceClient : IDisposable
{
    private const string AuthorizationUrl = "https://login.xero.com/identity/connect/authorize";

    private const string TokenUrl = "https://identity.xero.com/connect/token";

    private const string Scopes = "offline_access openid profile email accounting.transactions accounting.settings";

    private const string RedirectUrl = "http://localhost:8888/callback";

    private const string State = "excalibur";

    private readonly ExcaliburConfiguration configuration;

    private readonly HttpListener listener = new() {Prefixes = {"http://localhost:8888"}};

    private readonly HttpClient client = new();

    public PkceClient(ExcaliburConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<Token?> GetToken()
    {
        if (!string.IsNullOrEmpty(configuration.RefreshToken))
        {
            var refresh = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", configuration.ClientId),
                new KeyValuePair<string, string>("refresh_token", configuration.RefreshToken)
            });

            var refreshResponse = await client.PostAsync(TokenUrl, refresh);

            if (refreshResponse.IsSuccessStatusCode)
            {
                return await refreshResponse.Content.ReadFromJsonAsync<Token>();
            }
        }

        var codeVerifier = GenerateCodeVerifier();
        var authorizationCode = await GetAuthorizationCode(codeVerifier);

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("client_id", configuration.ClientId),
            new KeyValuePair<string, string>("code", authorizationCode),
            new KeyValuePair<string, string>("redirect_uri", RedirectUrl),
            new KeyValuePair<string, string>("code_verifier", codeVerifier),
        });

        var response = await client.PostAsync(TokenUrl, content);

        return await response.Content!.ReadFromJsonAsync<Token>();
    }

    private async Task<string> GetAuthorizationCode(string codeVerifier)
    {
        var link = GenerateLink(codeVerifier);
        Process.Start(link);

        listener.Start();

        var code = await GetResponse();

        listener.Stop();

        return code;
    }

    public void Dispose()
    {
        listener.Stop();
    }

    private async Task<string> GetResponse()
    {
        var context = await listener.GetContextAsync();

        if (context.Request.Url?.AbsolutePath != "/callback")
        {
            throw new InvalidOperationException("Invalid callback url specified for Xero authorization");
        }

        var query = context.Request.Url.Query;

        if (!ParseQuery(context.Request.Url.Query, out var code, out var state))
        {
            throw new InvalidOperationException("Missing authorization code in response from Xero");
        }

        if (state != State)
        {
            throw new InvalidOperationException("Insecure interception detected from Xero");
        }

        WriteCompletion(context.Response);

        return code;
    }

    private bool ParseQuery(string query, out string code, out string state)
    {
        if (query.Contains("?"))
        {
            query = query.Substring(query.IndexOf('?') + 1);
        }

        code = string.Empty;
        state = string.Empty;

        foreach (var part in query.Split('&'))
        {
            var pair = part.Split('=');

            if (pair.Length == 2)
            {
                switch (pair[0])
                {
                    case "code":
                        code = pair[1];
                        break;

                    case "state":
                        state = pair[1];
                        break;
                }
            }
        }

        return !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state);
    }

    private void WriteCompletion(HttpListenerResponse response)
    {
        var buffer = Encoding.UTF8.GetBytes("You can close this window now");

        response.ContentType = "text/html";
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);
    }

    private string GenerateLink(string codeVerifier)
    {
        var scopes = Uri.EscapeDataString(Scopes);
        var challenge = GenerateChallenge(codeVerifier);

        return $"{AuthorizationUrl}?response_type=code&client_id={configuration.ClientId}&redirect_uri={RedirectUrl}&scope={scopes}&state={State}&code_challenge={challenge}&code_challenge_method=S256";
    }

    private string GenerateCodeVerifier()
    {
        var generator = RandomNumberGenerator.Create();

        var buffer = new byte[32];
        generator.GetBytes(buffer);

        return Convert.ToBase64String(buffer).TrimChallenge();
    }

    private string GenerateChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();

        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

        return Convert.ToBase64String(challengeBytes).TrimChallenge();
    }
}
