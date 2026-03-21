using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Notifications

{

	public class NotificationUpdateDto

	{

		[Required]

		public NotificationStatus Status { get; set; }

	}

}
