using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "WTYEGFDSAHXCBVFGDSJACBVGFWEYUDAHK";
        private const int JWT_TOKEN_VALIDITY_MINS = 20;
        private readonly List<UserAccount> _userAccount;

        public JwtTokenHandler()
        {
            _userAccount = new List<UserAccount>
            {
                new UserAccount{username="admin", password="admin123",role = "Admin" },
                new UserAccount{username="user1", password="user1",role = "User" },
                new UserAccount{username="user2", password="user2",role = "User" },
            };
        }
        
        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            /*Validation*/

            var user = _userAccount.Where(x => x.username == request.UserName && x.password == request.Password).FirstOrDefault();
            if(user == null) return null;
            var tokenExpiryTime = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, request.UserName),
                new Claim("Role", user.role)
            });

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256
                );
            var securitytokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTime,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securitytokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = request.UserName,
                ExpiresIn = (int)tokenExpiryTime.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token,
            };
        }
    }
}
