namespace WarehousePro.API.DTOs.Item

{

	public class ItemResponseDto

	{

		public int ItemID { get; set; }

		public string SKU { get; set; } = string.Empty;

		public string? Description { get; set; }

		public string UnitOfMeasure { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; }

		public int TotalQuantityOnHand { get; set; }

		public int TotalReservedQuantity { get; set; }

		public int TotalAvailableQuantity { get; set; }

	}

}
