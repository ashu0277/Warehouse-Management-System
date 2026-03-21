using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.User

{

	public class UserUpdateDto

	{

		[Required]

		[MaxLength(100)]

		public string Name { get; set; } = string.Empty;

		[Required]

		public UserRole Role { get; set; }

		[MaxLength(20)]

		public string? Phone { get; set; }

		[Required]

		public bool IsDeleted { get; set; }

	}

}
