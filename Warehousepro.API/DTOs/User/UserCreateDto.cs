using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.User

{

	public class UserCreateDto

	{

		[Required]

		[MaxLength(100)]

		public string Name { get; set; } = string.Empty;

		[Required]

		public UserRole Role { get; set; }

		[Required]

		[EmailAddress]

		[MaxLength(150)]

		public string Email { get; set; } = string.Empty;

		[MaxLength(20)]

		public string? Phone { get; set; }

		[Required]

		[MinLength(8)]

		public string Password { get; set; } = string.Empty;

		[Required]

		[Compare("Password")]

		public string ConfirmPassword { get; set; } = string.Empty;

	}

}
