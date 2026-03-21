using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Outbound
{
	public class ShipmentUpdateDto
	{
		[Required]
		public ShipmentStatus Status { get; set; }

		public DateTime? DispatchDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
	}
}