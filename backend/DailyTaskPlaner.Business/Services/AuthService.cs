using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Common;
using DailyTaskPlaner.Common.DTOs;
using DailyTaskPlaner.Data;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DailyTaskPlaner.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        
        private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromMinutes(30);

        public AuthService(AppDbContext context, IConfiguration configuration, PasswordHasher<User> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginUserDto request)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            string token = CreateToken(user);

            RefreshToken refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();

            LoginResponseDto response = new LoginResponseDto
            {
                UserId = user.Id,
                AccessToken = token,
                RefreshToken = refreshToken.Token
            };

            return response;
        }

        public async Task<ResultPackage<bool>> LogoutAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token is null)
            {
                return new ResultPackage<bool>(ResultStatus.BadRequest, "Invalid refresh token");
            }

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();

            return new ResultPackage<bool>(true);
        }

        public async Task<ResultPackage<TokenResponseDto?>> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            RefreshToken? refreshToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                return new ResultPackage<TokenResponseDto?>(ResultStatus.BadRequest, "Invalid refresh token");
            }

            string accessToken = CreateToken(refreshToken.User);

            refreshToken.Token = GenerateRefreshToken();
            refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            TokenResponseDto response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

            return new ResultPackage<TokenResponseDto?>(response);
        }

        public async Task<ResultPackage<User>> RegisterAsync(UserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new ResultPackage<User>(ResultStatus.BadRequest, "Username and password are required");
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return new ResultPackage<User>(ResultStatus.Conflict, "Username already exists");
            }

            User user = new User
            {
                Name = request.Name,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return new ResultPackage<User>(user, ResultStatus.Created, "User registered successfully");
            }
            catch (DbUpdateException ex)
            {
                return new ResultPackage<User>(ResultStatus.InternalServerError, "Database error occurred while registering user");
            }
            catch (Exception ex)
            {
                return new ResultPackage<User>(ResultStatus.InternalServerError, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Issues a permit to set a new password and returns the raw token, which the
        /// caller puts into the link it mails out.
        ///
        /// The password itself is left alone. It changes only in SetNewPasswordAsync,
        /// once the user proves the mail reached them, so a mail that never arrives
        /// leaves the account exactly as it was.
        /// </summary>
        public async Task<ResultPackage<string>> RequestPasswordResetAsync(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                return new ResultPackage<string>(ResultStatus.NotFound,
                                                 "No user found with that email address.");
            }

            // Any permit issued earlier is spent, so the newest link is the only one that works.
            var pending = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && t.UsedOnUtc == null)
                .ToListAsync();

            foreach (var token in pending)
            {
                token.UsedOnUtc = DateTime.UtcNow;
            }

            string rawToken = GenerateUrlSafeToken();

            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = HashToken(rawToken),
                ExpiresOnUtc = DateTime.UtcNow.Add(ResetTokenLifetime)
            });

            await _context.SaveChangesAsync();

            return new ResultPackage<string>(rawToken, ResultStatus.OK,
                                             "Password reset link created.");
        }

        /// <summary>
        /// Spends the token and sets the new password. Refusals say nothing about why,
        /// so an unknown, spent and expired token look the same from the outside.
        /// </summary>
        public async Task<ResultPackage<bool>> SetNewPasswordAsync(SetNewPasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new ResultPackage<bool>(ResultStatus.BadRequest,
                                               "Token and new password are required.");
            }

            string tokenHash = HashToken(request.Token);

            PasswordResetToken? resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

            if (resetToken is null || resetToken.UsedOnUtc is not null
                                   || resetToken.ExpiresOnUtc <= DateTime.UtcNow)
            {
                return new ResultPackage<bool>(ResultStatus.BadRequest,
                                               "This password reset link is no longer valid.");
            }

            resetToken.User.PasswordHash = _passwordHasher.HashPassword(resetToken.User, request.NewPassword);
            resetToken.UsedOnUtc = DateTime.UtcNow;

            // Sessions opened with the old password end here, since whoever changed the
            // password may be locking someone else out on purpose.
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == resetToken.UserId)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(refreshTokens);

            await _context.SaveChangesAsync();

            return new ResultPackage<bool>(true, ResultStatus.OK, "Password changed successfully.");
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"])); // Use indexer syntax to access configuration values

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"], // Use indexer syntax here as well
                audience: _configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        /// <summary>256 bits of randomness, encoded so it survives being part of a URL.</summary>
        private static string GenerateUrlSafeToken()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        }

        /// <summary>
        /// The database keeps only this. A plain hash is enough here, unlike for passwords,
        /// because the token is long and random rather than something a person made up.
        /// </summary>
        private static string HashToken(string rawToken)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(hash);
        }
    }
}
