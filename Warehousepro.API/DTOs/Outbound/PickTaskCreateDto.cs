using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Outbound
{
	public class PickTaskCreateDto
	{
		[Required]
		public int OrderID { get; set; }

		[Required]
		public int ItemID { get; set; }

		[Required]
		public int BinID { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Pick quantity must be at least 1")]
		public int PickQuantity { get; set; }

		public int? AssignedToUserID { get; set; }
	}
}