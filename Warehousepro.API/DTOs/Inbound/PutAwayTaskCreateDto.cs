using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Inbound
{
	public class PutAwayTaskCreateDto
	{
		[Required]
		public int ReceiptID { get; set; }

		[Required]
		public int ItemID { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
		public int Quantity { get; set; }

		[Required]
		public int TargetBinID { get; set; }

		public int? AssignedToUserID { get; set; }
	}
}