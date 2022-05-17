using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Ecoba.AzureAuth
{
    public class AzureAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfig _config;
        public AzureAuthMiddleware(RequestDelegate next, JwtConfig config)
        {
            _next = next;
            _config = config;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("azure-token", out var token))
            {
                var end_point = "https://graph.microsoft.com/v1.0/me?$select=displayName,givenName,jobTitle,mail,mobilePhone,officeLocation,preferredLanguage,surname,userPrincipalName,id,employeeID";
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _client.GetAsync(end_point);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsAsync<AzureResponse>();
                    context.Request.Headers.Add("Authorization", $"Bearer {GenerateToken(res)}");
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        private string GenerateToken(AzureResponse res)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, res.EmployeeId),
            new Claim(ClaimTypes.Email, res.Mail),
            new Claim(ClaimTypes.Name, res.DisplayName)
        };
            var token = new JwtSecurityToken(_config.Issuer,
                _config.Audience,
                claims,
                expires: DateTime.Now.AddHours(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}