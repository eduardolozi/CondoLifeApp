using System.Security.Claims;
using System.Security.Cryptography;
using API.Handlers;
using API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API {
    public static class ApiInjectionModule {
        public static void AddApiServices(this IServiceCollection services) {
            services.AddControllers();
            services.ConfigureAuthentication();
            services.AddPolicies();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddProblemDetails();
            services.AddSignalR();
            services.AddExceptionHandler<ExceptionHandler>();
            services.AddScoped<EmailNotificationHub>();
        }

        private static void ConfigureAuthentication(this IServiceCollection services) {
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String("MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAKwOcnAOqwp/1szNR84m3km7RI8A2XNDNIvs7ucneU+kOLiKv5yJ3LWqhlXH/6ZmPDE17l8sqoeBX21jb8KdebfxECi8I9m1j2sHWzHVejA7iNQJ6wcZOlhuswKa1+tdwQ/i9GMIQfI7PjXCwdMNDScZYkAYcChviXNWR4Mx3u7DAgMBAAECgYEAiQjUomU8WxdomCNjblDMuIK7Xv45MrEzF8L0oAxzdTgBqRFw/RdcPyB677Vj6z7/793ZZdooU9Z5j6Ej8SgFOWmmoS6HWq2tNSZEPROY4um9/XkqkrFUdMTZCHUWLGbubeIYDVF16DZ25+v+C7+/yjXtJrUjBJUzuIO6/wOfwqECQQDdzAt1t0r/v0RI19kg++iXR79mqGwr3ReCD+SowqrltLW7gOTirVkG5525wqCkTFddb2U8fiPZOsuZ/U0qwmKRAkEAxpbGiw9SbIpceHrYZ3upys8W2VE7KFcRjOM95Jexv2FMiCD/QpMiNG+OThx2m3LEMz+uRV5u5Vs/F0kQjKq+EwJAPwyv/Uibk1QFz0c8u/mgRtDoggBCr71r31cxQyADgMT8HE8pwZ5Rfnr9BT9kdxAUjcUK3EVnX2stUZsGAq+7YQJAH9/deD56VU+T7gaRq3Ju202H9lOScjQfbgSfT4yFjBk65nKdZfslt1LcfW8WHnc6RJuJBjtVA1008DDbBij1nwJAHy3fmdZ/qnN2scRlo+VztSrtSnUpdbeLVZpPqGn8uEUxezMGmgsjZi0zYUf4HBPfdeUescUn4SncEOhlTIVc/Q=="), out _);
            var rsaKey = new RsaSecurityKey(rsa);

            //env
            services
            .AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "https://localhost:7031",
                    ValidAudience = "https://localhost:7031",
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        private static void AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"))
                .AddPolicy("ManagerOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Manager"))
                .AddPolicy("AdminOrManagerOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "Manager"));
        }


    } 
}
