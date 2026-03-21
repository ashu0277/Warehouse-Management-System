using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Outbound
{
	public class ShipmentCreateDto
	{
		[Required]
		public int OrderID { get; set; }

		[Required]
		[MaxLength(100)]
		public string Carrier { get; set; } = string.Empty;

		public DateTime? DispatchDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
	}
}