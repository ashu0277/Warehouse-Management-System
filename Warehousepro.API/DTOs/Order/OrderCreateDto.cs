using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Order
{
	public class OrderCreateDto
	{
		[Required]
		[MaxLength(100)]
		public string OrderNumber { get; set; } = string.Empty;

		[Required]
		[MaxLength(150)]
		public string CustomerName { get; set; } = string.Empty;

		[MaxLength(300)]
		public string? DeliveryAddress { get; set; }

		[Required]
		public DateTime OrderDate { get; set; }

		public DateTime? RequiredDate { get; set; }
	}
}