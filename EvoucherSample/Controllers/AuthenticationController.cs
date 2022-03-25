using EvoucherSample.DataService;
using EvoucherSample.Models;
using EvoucherSample.Models.Auth;
using EvoucherSample.Models.AuthViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace EvoucherSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AuthenticationUser> userManager;
        private readonly RoleManager<AuthenticationRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly BackendDbContext context;

        public AuthenticationController
        (
            UserManager<AuthenticationUser> _userManager,
            RoleManager<AuthenticationRole> _roleManager,
            BackendDbContext _context,
            IConfiguration _configuration
        )
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            configuration = _configuration;
            context = _context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(1), // token will be expiry in 24 hours ( 1 day )
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var refreshToken = generateRefreshToken(model.Email);

                await context.refreshTokens.AddAsync(refreshToken);

                context.Update(user);
                
                await context.SaveChangesAsync();

                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    username = user.UserName,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    expiration = token.ValidTo
                });
            }
            return Unauthorized(new
            {
                status = 419,
                message = "User Not Found"
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserModel model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            AuthenticationUser user = new AuthenticationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { 
                    Status = "Error", 
                    Message = "User creation failed! Please check user details and try again."
                });

            return Ok(new { Status = "Success", Message = "User created successfully! Plz Login Again." });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null) return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "No User Found" });
            //(u => u.RefreshTokens.Any(t => t.Token == token));

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(email);

            refreshToken.Revoked = DateTime.UtcNow;

            refreshToken.RevokedBy = email;

            refreshToken.ReplacedByToken = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            context.Update(user);
            context.SaveChanges();

            // generate new jwt
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var newToken = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1), // token will be expiry in 24 hours ( 1 day )
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                username = user.UserName,
                token = new JwtSecurityTokenHandler().WriteToken(newToken),
                RefreshToken = refreshToken,
                expiration = newToken.ValidTo
            });
        }


        private RefreshToken generateRefreshToken(string email)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedBy = email
                };
            }
        }
    }
}
