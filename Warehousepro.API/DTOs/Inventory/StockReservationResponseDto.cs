namespace WarehousePro.API.DTOs.Inventory
{
	public class StockReservationResponseDto
	{
		public int ReservationID { get; set; }
		public int ItemID { get; set; }
		public string SKU { get; set; } = string.Empty;
		public string ReferenceType { get; set; } = string.Empty;
		public int ReferenceID { get; set; }
		public int Quantity { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}