using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Inbound

{

	public class PutAwayTaskUpdateDto

	{

		public int? AssignedToUserID { get; set; }

		[Required]

		public PutAwayStatus Status { get; set; }

		public int? TargetBinID { get; set; }

	}

}
