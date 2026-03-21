using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Replenishment
{
	public class ReplenishmentTaskCreateDto
	{
		[Required]
		public int ItemID { get; set; }

		[Required]
		public int FromBinID { get; set; }

		[Required]
		public int ToBinID { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
		public int Quantity { get; set; }
	}
}