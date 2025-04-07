using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RZD.Application.Models;
using RZD.Common.Configs;
using RZD.Common.Exceptions;
using RZD.Database;
using RZD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using RZD.Application.Helpers;


namespace RZD.Application.Services
{
    public class AuthService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly DataContext _context;

        public AuthService(IOptions<JwtConfig> options, DataContext context)
        {
            _jwtConfig = options.Value;
            _context = context;
        }

        public async Task<TokensModel> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new UnauthorizedExeption();
            }

            var accessToken = await GenerateAccessTokenAsync(user.Id);
            var refreshToken = GenerateRefreshToken();

            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new TokensModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                throw new BadRequestExeption("User already exists.");
            }

            var hashedPassword = PasswordHasher.HashPassword(request.Password);
            await _context.Users.AddAsync(new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                CreatedDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task<TokensModel> RefreshAsync(RefreshRequest request)
        {
            var storedToken = await _context.RefreshTokens.Where(x => x.UserId == request.UserId).FirstOrDefaultAsync();
            if (storedToken == null || storedToken.Token != request.RefreshToken)
            {
                throw new UnauthorizedExeption();
            }

            if (storedToken.ExpiryDate < DateTimeOffset.Now)
            {
                throw new UnauthorizedExeption("Refresh token expired.");
            }

            var newAccessToken = await GenerateAccessTokenAsync(request.UserId);
            var newRefreshToken = GenerateRefreshToken();

            await SaveRefreshTokenAsync(request.UserId, newRefreshToken);

            return new TokensModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        private async Task<string> GenerateAccessTokenAsync(long userId)
        {
            var email = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Email)
                .FirstOrDefaultAsync();

            var roles = await(from ur in _context.UserRoles
                              join r in _context.Roles on ur.RoleId equals r.Id
                              where ur.UserId == userId
                              select r.Name).ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
            };

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(_jwtConfig.AccessExpiredIn),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SaveRefreshTokenAsync(long userId, string refreshToken)
        {
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
            if (existingToken != null)
            {
                existingToken.Token = refreshToken;
                existingToken.ExpiryDate = DateTimeOffset.UtcNow.AddSeconds(_jwtConfig.RefreshExpiredIn);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.RefreshTokens.AddAsync(new RefreshToken
                {
                    UserId = userId,
                    Token = refreshToken,
                    ExpiryDate = DateTimeOffset.UtcNow.AddSeconds(_jwtConfig.RefreshExpiredIn)
                });
                await _context.SaveChangesAsync();
            }
        }

        private string GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            return refreshToken;
        }
    }
}
