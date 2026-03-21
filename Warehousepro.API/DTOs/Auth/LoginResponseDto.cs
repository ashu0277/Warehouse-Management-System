using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Auth

{



	// ──────────────────────────────────────────────────────────────────────

	// What the API sends back after successful login

	// ──────────────────────────────────────────────────────────────────────

	public class LoginResponseDto

	{

		public string Token { get; set; } = string.Empty;

		public string TokenType { get; set; } = "Bearer";

		public DateTime ExpiresAt { get; set; }

		public int UserID { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string Role { get; set; } = string.Empty;

	}
}