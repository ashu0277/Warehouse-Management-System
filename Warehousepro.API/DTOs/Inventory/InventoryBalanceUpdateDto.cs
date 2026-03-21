using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Inventory

{

	public class InventoryBalanceUpdateDto

	{

		[Required]

		[Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]

		public int QuantityOnHand { get; set; }

		[Required]

		[Range(0, int.MaxValue)]

		public int ReservedQuantity { get; set; }

	}

}
