namespace WarehousePro.API.DTOs.Outbound
{
	public class PickTaskResponseDto
	{
		public int PickTaskID { get; set; }
		public int OrderID { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public int ItemID { get; set; }
		public string SKU { get; set; } = string.Empty;
		public string? ItemDescription { get; set; }
		public int BinID { get; set; }
		public string BinCode { get; set; } = string.Empty;
		public string ZoneName { get; set; } = string.Empty;
		public int PickQuantity { get; set; }
		public int? AssignedToUserID { get; set; }
		public string? AssignedToName { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
	}
}