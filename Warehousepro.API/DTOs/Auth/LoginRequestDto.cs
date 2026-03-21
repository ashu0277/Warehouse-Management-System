using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Auth

{

	// ──────────────────────────────────────────────────────────────────────

	// What the client sends when logging in

	// ──────────────────────────────────────────────────────────────────────

	public class LoginRequestDto

	{

		[Required]

		[EmailAddress]

		public string Email { get; set; } = string.Empty;

		[Required]

		public string Password { get; set; } = string.Empty;

	}
}