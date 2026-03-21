using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Order

{

	public class OrderUpdateDto

	{

		[MaxLength(300)]

		public string? DeliveryAddress { get; set; }

		public DateTime? RequiredDate { get; set; }

		[Required]

		public OrderStatus Status { get; set; }

	}

}
