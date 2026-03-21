using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Notifications

{

	public class NotificationCreateDto

	{

		[Required]

		public int UserID { get; set; }

		[Required]

		[MaxLength(500)]

		public string Message { get; set; } = string.Empty;

		[Required]

		public NotificationCategory Category { get; set; }

	}

}
