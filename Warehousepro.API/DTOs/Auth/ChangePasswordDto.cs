using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Auth

{

	
	
	

	// ──────────────────────────────────────────────────────────────────────

	// What the client sends when changing password

	// ──────────────────────────────────────────────────────────────────────

	public class ChangePasswordDto

	{

		[Required]

		public string CurrentPassword { get; set; } = string.Empty;

		[Required]

		[MinLength(8)]

		public string NewPassword { get; set; } = string.Empty;

		[Required]

		[Compare("NewPassword")]

		public string ConfirmNewPassword { get; set; } = string.Empty;

	}

}
