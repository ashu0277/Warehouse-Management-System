using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Inbound
{
	public class InboundReceiptCreateDto
	{
		[Required]
		[MaxLength(100)]
		public string ReferenceNo { get; set; } = string.Empty;

		[Required]
		[MaxLength(150)]
		public string Supplier { get; set; } = string.Empty;

		[Required]
		public DateTime ReceiptDate { get; set; }
	}
}