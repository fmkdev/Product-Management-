using System;
using API.Domain.Entities;
using API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace API.Jwt
{
    public class Jwt : IJwt
    {
        private readonly IConfiguration _config;
        private IJwtFactory _jwtFactory;
        private UserManager<User> _userManager;

        public Jwt(IConfiguration config, RoleManager<Role> roleManager, UserManager<User> userManager, IJwtFactory jwtFactory)
        {
            _config = config;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
        }

        public AuthTokenDto Generate(User user)
        {
            var token = _jwtFactory.GenerateEncodedToken(user).Result;

            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpireDate = DateTime.UtcNow.AddDays(14);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresDate = DateTime.UtcNow.AddDays(14);

            var x = _userManager.UpdateAsync(user).Result;

            return new AuthTokenDto()
            {
                Token = token,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpireDate
            };

        }
    }
}