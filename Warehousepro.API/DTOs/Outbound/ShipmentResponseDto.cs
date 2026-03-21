namespace WarehousePro.API.DTOs.Outbound
{
	public class ShipmentResponseDto
	{
		public int ShipmentID { get; set; }
		public int OrderID { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public string CustomerName { get; set; } = string.Empty;
		public string Carrier { get; set; } = string.Empty;
		public DateTime? DispatchDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
	}
}