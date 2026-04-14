using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Auth;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class AuthService : IAuthService

	{

		private readonly AppDbContext _context;

		private readonly IConfiguration _configuration;

		private readonly IAuditLogService _auditLogService;

		public AuthService(

			AppDbContext context,

			IConfiguration configuration,

			IAuditLogService auditLogService)

		{

			_context = context;

			_configuration = configuration;

			_auditLogService = auditLogService;

		}

		// ── Login ─────────────────────────────────────────────────────────

		public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)

		{


		

			var user = await _context.Users

				.FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);

			if (user == null) return null;

			// Plain text password comparison

			if (user.PasswordHash != dto.Password)

				return null;

			var token = GenerateJwtToken(user.UserID, user.Email, user.Role.ToString());

			var expiresAt = DateTime.UtcNow.AddDays(

				int.Parse(_configuration["JwtSettings:ExpiryInDays"] ?? "7"));

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: user.UserID,

				action: "LOGIN",

				resource: "Auth",

				metadata: $"Email: {user.Email}, Role: {user.Role}"

			);

			return new LoginResponseDto

			{

				Token = token,

				TokenType = "Bearer",

				ExpiresAt = expiresAt,

				UserID = user.UserID,

				Name = user.Name,

				Email = user.Email,

				Role = user.Role.ToString()

			};

		}

		// ── Change Password ───────────────────────────────────────────────

		public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)

		{

			var user = await _context.Users

				.FirstOrDefaultAsync(u => u.UserID == userId && !u.IsDeleted);

			if (user == null) return false;

			// Plain text password comparison

			if (user.PasswordHash != dto.CurrentPassword)

				return false;

			// Store new password as plain text

			user.PasswordHash = dto.NewPassword;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: userId,

				action: "CHANGE_PASSWORD",

				resource: "Auth",

				metadata: $"Email: {user.Email}"

			);

			return true;

		}

		// ── JWT Token Generator ───────────────────────────────────────────

		private string GenerateJwtToken(int userId, string email, string role)

		{

			var secretKey = _configuration["JwtSettings:SecretKey"]!;

			var issuer = _configuration["JwtSettings:Issuer"]!;

			var audience = _configuration["JwtSettings:Audience"]!;

			var expiryDays = int.Parse(_configuration["JwtSettings:ExpiryInDays"] ?? "7");

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]

			{

				new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),

				new Claim(JwtRegisteredClaimNames.Email, email),

				new Claim(ClaimTypes.Role,               role),

				new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())

			};

			var token = new JwtSecurityToken(

				issuer: issuer,

				audience: audience,

				claims: claims,

				expires: DateTime.UtcNow.AddDays(expiryDays),

				signingCredentials: creds

			);

			return new JwtSecurityTokenHandler().WriteToken(token);

		}

	}

}
