using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Ecoba.AzureAuth
{
    public static class AzureAuthSchemeExtension
    {
        public static void AddAzureAuthScheme(this IServiceCollection services, JwtConfig config)
        {
            services.AddSingleton(config);
            
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
             {
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = config.Issuer,
                     ValidAudience = config.Audience,
                     IssuerSigningKeys = new[]{
             new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key)),
             new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.KeyService)),
                     },
                 };
             });
        }
    }
}