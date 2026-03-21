namespace WarehousePro.API.DTOs.User

{

	public class UserResponseDto

	{

		public int UserID { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Role { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string? Phone { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime CreatedAt { get; set; }

	}

}
