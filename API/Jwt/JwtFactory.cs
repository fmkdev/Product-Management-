using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Jwt
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<JwtFactory> _logger;
        

        public JwtFactory(JwtIssuerOptions jwtOptions, UserManager<User> userManager, IConfiguration config, ILogger<JwtFactory> logger)
        {
            _jwtOptions = jwtOptions;
            _userManager = userManager;
           
            _config = config;
  
            _logger = logger;
  

            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<string> GenerateEncodedToken(User user)
        {
            var identity = await GenerateClaimsIdentity(user);
            var claims = new List<Claim>
              {
                  new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
              };
            claims.AddRange(identity.Claims);
            // Create the JWT security token and encode it.
           
            var jwt = new JwtSecurityToken(
                issuer: _config.GetValue<string>("JwtIssuerOptions.Issuer"),
                audience:  _config.GetValue<string>("JwtIssuerOptions.Audience"),
                
                claims: claims,
                // notBefore: _jwtOptions.NotBefore,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: _jwtOptions.SigningCredentials
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

  
  public async Task<ClaimsIdentity> GenerateClaimsIdentity(User user)
        {
               
      
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), new[]
             {
                 new Claim(ClaimTypes.NameIdentifier, user.UserName),
                 new Claim(ClaimTypes.Email, user.Email),
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                 new Claim("uid", user.Id.ToString()),
                 new Claim("id", user.Id??""),
                new Claim("username", user.UserName??""),
                new Claim("email", user.Email??""),
                new Claim("isActive", user.IsActive.ToString()??""),
                new Claim("clientId", "" ),
                new Claim("tokenType", "user"),
             }); 
                      

            var roles = await _userManager.GetRolesAsync(user);

            if (roles != null)
            {
                if (roles.Count() != 0)
                {
                    foreach (var i in roles)
                    {
                        claimsIdentity.AddClaim(new Claim("roles", i));
                    }
                }
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            claimsIdentity.AddClaims(userClaims);
         
            return claimsIdentity;
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }
        }


    }
}