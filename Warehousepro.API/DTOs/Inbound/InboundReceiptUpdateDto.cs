using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Inbound
{
	public class InboundReceiptUpdateDto
	{
		[Required]
		public ReceiptStatus Status { get; set; }
	}
}