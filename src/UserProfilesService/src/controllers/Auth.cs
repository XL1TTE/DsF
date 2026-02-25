
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Controllers;

[ApiController, Route("Auth")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    private readonly HttpClient _httpClient = new();

    [HttpGet("token")]
    public async Task<ActionResult<string>> ReciveToken([FromQuery(Name = "code")] string code)
    {
        var tokenEndpoint = configuration["Identity:TokenEndpoint"]
            ?? throw new InvalidOperationException("Identity:TokenEndpoint not configured");
        var clientId = configuration["Identity:ClientId"]
            ?? throw new InvalidOperationException("Identity:ClientId not configured");
        var redirectUri = configuration["Identity:RedirectUri"]
            ?? throw new InvalidOperationException("Identity:RedirectUri not configured");

        // Исправляем localhost на https для TokenEndpoint
        var tokenUri = tokenEndpoint.Replace("localhost", "host.docker.internal");

        var request = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("client_id", clientId),
        });

        request.Headers.Allow.Add("application/json");

        var response = await _httpClient.PostAsync(tokenUri, request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest($"Failed to exchange code for token: {responseContent}");
        }

        using var doc = JsonDocument.Parse(responseContent);
        var accessToken = doc.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("access_token not found in response");

        return Ok(accessToken);
    }
}
