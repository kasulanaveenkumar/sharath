using Common.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Common.Authorization.Authorize
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(TokenParams param, int tokenLifeTime);
        public TokenParams ValidateJwtToken(string token, bool isAccessToken, bool isRefreshToken);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }

    public class JwtUtils : IJwtUtils
    {
        // private readonly Models.TokenSettings _appSettings;
        private string SecretKey = "IQ&d(II[lW;T?~+~T;+g[Ni2+38l:Bq[qn4a@+F?z%Ood&v#9#NAc=USPaCngz";

        public JwtUtils()
        {

        }

        public JwtUtils(string secretkey)
        {
            SecretKey = secretkey;
            // _appSettings = new Models.TokenSettings() { Secret = "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING", RefreshTokenTTL = 120 };
        }

        //public JwtUtils(Models.TokenSettings appSettings)
        //{
        //    _appSettings = appSettings;
        //}

        public string GenerateJwtToken(TokenParams param, int tokenLifeTime)
        {
            // generate token that is valid for 1 hour
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                                {
                                    new Claim("TokenType", "AccessToken"),
                                    new Claim("UserId", param.UserId.ToString()),
                                    new Claim("CompanyGuid", param.CompanyGuid),
                                    new Claim("UserGuid", param.UserGuid),
                                    new Claim("UserTypeId", param.UserTypeId.ToString()),
                                    new Claim("UserRoles", (param.UserRoles != null && param.UserRoles.Length > 0) ? string.Join(",", param.UserRoles) : "")
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenLifeTime),
                //Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(TokenParams param, int tokenLifeTime)
        {
            // generate token that is valid for 1 hour
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                                {
                                    new Claim("TokenType", "RefreshToken"),
                                    new Claim("UserId", param.UserId.ToString()),
                                    new Claim("CompanyGuid", param.CompanyGuid),
                                    new Claim("UserGuid", param.UserGuid),
                                    new Claim("UserTypeId", param.UserTypeId.ToString()),
                                    new Claim("UserRoles", (param.UserRoles != null && param.UserRoles.Length > 0) ? string.Join(",", param.UserRoles) : "")
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenLifeTime),
                //Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token">Token Value</param>
        /// <param name="isVerifyTokenType">True - validate token type | False - Skip token type validation</param>
        /// <returns></returns>
        public TokenParams ValidateJwtToken(string token, bool isAccessToken, bool isRefreshToken)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var tokenType = jwtToken.Claims.First(x => x.Type == "TokenType").Value;
                var userId = long.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);
                var companyGuid = jwtToken.Claims.First(x => x.Type == "CompanyGuid").Value;
                var userGuid = jwtToken.Claims.First(x => x.Type == "UserGuid").Value;
                var userTypeId = long.Parse(jwtToken.Claims.First(x => x.Type == "UserTypeId").Value);
                var userRoles = jwtToken.Claims.First(x => x.Type == "UserRoles").Value;

                // Access token should have token type as "AccessToken"
                if (isAccessToken && tokenType != "AccessToken") 
                    return null;

                // Access token should have token type as "AccessToken"
                if (isRefreshToken && tokenType != "RefreshToken")
                    return null;

                // return user id from JWT token if validation successful
                return new TokenParams()
                {
                    //TokenType = tokenType,
                    CompanyGuid = companyGuid,
                    UserId = userId,
                    UserGuid = userGuid,
                    UserTypeId = userTypeId,
                    UserRoles = !string.IsNullOrEmpty(userRoles) ? userRoles.Split(',').Select(int.Parse).ToArray() : null
                };
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            // generate token that is valid for 7 days
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;
        }
    }
}
