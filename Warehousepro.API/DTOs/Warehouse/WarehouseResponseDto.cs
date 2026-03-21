namespace WarehousePro.API.DTOs.Warehouse
{
	public class WarehouseResponseDto
	{
		public int WarehouseID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public int TotalZones { get; set; }
	}
}