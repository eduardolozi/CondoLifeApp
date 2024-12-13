using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Infraestructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Raven.Client.Documents;
using User = Domain.Models.User;

namespace Application.Services {
    public class AuthService : IAuthService {
        private readonly CondoLifeContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(CondoLifeContext dbContext, IDocumentStore ravenStore, UserService userService)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<User>();
        }

        public LoginResponseDTO? Login(UserLoginDTO userLogin) {
            var user = _dbContext.Users.Include(x => x.Condominium).FirstOrDefault(x => x.Email.ToLower() == userLogin.Email.ToLower());
            if(user is null) return new LoginResponseDTO { IsSuccess = false, ErrorMessage = "Email incorreto" };

            var verifyPasswordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, userLogin.Password);
            if(verifyPasswordResult == PasswordVerificationResult.Success) {
                return new LoginResponseDTO {
                    IsSuccess = true,
                    AccessToken = CreateAccessToken(user),
                    RefreshToken = CreateRefreshToken(user.Id).Token,
                    UserId = user.Id
                };
            }

            return new LoginResponseDTO { IsSuccess = false, ErrorMessage = "Senha incorreta" };
        }

        public string CreateAccessToken(User user) {
            //env
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String("MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAKwOcnAOqwp/1szNR84m3km7RI8A2XNDNIvs7ucneU+kOLiKv5yJ3LWqhlXH/6ZmPDE17l8sqoeBX21jb8KdebfxECi8I9m1j2sHWzHVejA7iNQJ6wcZOlhuswKa1+tdwQ/i9GMIQfI7PjXCwdMNDScZYkAYcChviXNWR4Mx3u7DAgMBAAECgYEAiQjUomU8WxdomCNjblDMuIK7Xv45MrEzF8L0oAxzdTgBqRFw/RdcPyB677Vj6z7/793ZZdooU9Z5j6Ej8SgFOWmmoS6HWq2tNSZEPROY4um9/XkqkrFUdMTZCHUWLGbubeIYDVF16DZ25+v+C7+/yjXtJrUjBJUzuIO6/wOfwqECQQDdzAt1t0r/v0RI19kg++iXR79mqGwr3ReCD+SowqrltLW7gOTirVkG5525wqCkTFddb2U8fiPZOsuZ/U0qwmKRAkEAxpbGiw9SbIpceHrYZ3upys8W2VE7KFcRjOM95Jexv2FMiCD/QpMiNG+OThx2m3LEMz+uRV5u5Vs/F0kQjKq+EwJAPwyv/Uibk1QFz0c8u/mgRtDoggBCr71r31cxQyADgMT8HE8pwZ5Rfnr9BT9kdxAUjcUK3EVnX2stUZsGAq+7YQJAH9/deD56VU+T7gaRq3Ju202H9lOScjQfbgSfT4yFjBk65nKdZfslt1LcfW8WHnc6RJuJBjtVA1008DDbBij1nwJAHy3fmdZ/qnN2scRlo+VztSrtSnUpdbeLVZpPqGn8uEUxezMGmgsjZi0zYUf4HBPfdeUescUn4SncEOhlTIVc/Q=="), out _);
            var rsaKey = new RsaSecurityKey(rsa);
            var signinCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

            var claims = new List<Claim> {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new ("Id", user.Id.ToString()),
                new (ClaimTypes.Name, user.Name),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.Role, user.Role.ToString()),
                new ("Apartment", $"{user.Apartment}"),
                new ("Block", user.Block ?? string.Empty),
                new ("CondominiumName", user.Condominium is null ? string.Empty : user.Condominium.Name),
                new ("PhotoUrl", user.PhotoUrl ?? string.Empty),
                new ("CondominiumId", user.CondominiumId.ToString()),
                new ("NotificationLifetime", user.NotificationLifetime is null ? "0" : user.NotificationLifetime.Value.ToString()),
                new ("NotifyEmail", user.NotifyEmail.ToString()),
                new ("NotifyPhone", user.NotifyPhone.ToString()),
            };

            var tokenOptions = new SecurityTokenDescriptor {
                //env
                Issuer = "https://localhost:7031",
                Audience = "https://localhost:7031",
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = signinCredentials
            };

            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtHandler.CreateJwtSecurityToken(tokenOptions);

            return jwtHandler.WriteToken(jwtSecurityToken);
        }

        public RefreshToken CreateRefreshToken(int userId) {
            var refreshToken = new RefreshToken {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = userId
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            _dbContext.SaveChanges();

            return refreshToken;
        }

        public ClaimsPrincipal GetClaimsPrincipalFromExpiredAccessToken(string accessToken) {
            var jwtHandler = new JwtSecurityTokenHandler();
            try {
                var jwtToken = jwtHandler.ReadToken(accessToken) as JwtSecurityToken
                    ?? throw new BadRequestException("Access Token vazio");

                return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, JwtBearerDefaults.AuthenticationScheme));
            }
            catch (Exception ex) {
                throw new BadRequestException("Erro ao obter claims principal do Access Token", ex);
            }
        }

        public LoginResponseDTO RefreshAccessToken(RefreshRequestDTO refreshRequest) {
            var refreshTokenInDb = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == refreshRequest.RefreshToken)
                ?? throw new Exception("Refresh Token não foi encontrado");
            var isRefreshTokenExpired = refreshTokenInDb.ExpiresAt < DateTime.UtcNow;

            if (!isRefreshTokenExpired) {
                var principal = GetClaimsPrincipalFromExpiredAccessToken(refreshRequest.AccessToken);

                var idClaim = principal.Claims.FirstOrDefault(x => x.Type == "Id")
                    ?? throw new BadRequestException("A claim Id não possui nenhum valor");
                var id = int.Parse(idClaim.Value); 

                var user = _dbContext.Users.FirstOrDefault(x => x.Id == id)
                    ?? throw new BadRequestException("O usuário não foi encontrado");

                return new LoginResponseDTO {
                    IsSuccess = true,
                    AccessToken = CreateAccessToken(user),
                    RefreshToken = refreshRequest.RefreshToken,
                };
            }

            return new LoginResponseDTO {
                IsSuccess = false,
                ErrorMessage = "Sessão expirada"
            };
        }

        public async Task DeleteExpiredRefreshTokens() {
            await _dbContext.RefreshTokens.Where(x => x.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync();
        }
    }
}
