using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Replenishment
{
	public class ReplenishmentTaskUpdateDto
	{
		// Keep these optional or required based on your business rules
		public int ItemID { get; set; }
		public int FromBinID { get; set; }
		public int ToBinID { get; set; }
		public int Quantity { get; set; }

		[Required]
		public ReplenishmentStatus Status { get; set; }
	}
}