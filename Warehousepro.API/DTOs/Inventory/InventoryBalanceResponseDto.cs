namespace WarehousePro.API.DTOs.Inventory
{
	public class InventoryBalanceResponseDto
	{
		public int BalanceID { get; set; }
		public int ItemID { get; set; }
		public string SKU { get; set; } = string.Empty;
		public string? ItemDescription { get; set; }
		public int BinID { get; set; }
		public string BinCode { get; set; } = string.Empty;
		public string ZoneName { get; set; } = string.Empty;
		public string WarehouseName { get; set; } = string.Empty;
		public int QuantityOnHand { get; set; }
		public int ReservedQuantity { get; set; }
		public int AvailableQuantity { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}