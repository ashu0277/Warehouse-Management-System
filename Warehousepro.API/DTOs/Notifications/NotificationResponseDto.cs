namespace WarehousePro.API.DTOs.Notifications
{
	public class NotificationResponseDto
	{
		public int NotificationID { get; set; }
		public int UserID { get; set; }
		public string Message { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
	}
}