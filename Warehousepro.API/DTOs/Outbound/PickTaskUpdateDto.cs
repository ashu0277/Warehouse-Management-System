using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Outbound

{

	public class PickTaskUpdateDto

	{

		public int? AssignedToUserID { get; set; }

		[Required]

		public PickTaskStatus Status { get; set; }

	}

}
