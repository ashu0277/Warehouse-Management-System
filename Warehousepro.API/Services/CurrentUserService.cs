using System.Security.Claims;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class CurrentUserService : ICurrentUserService

	{

		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)

		{

			_httpContextAccessor = httpContextAccessor;

		}

		public int GetUserId()

		{

			var claim = _httpContextAccessor.HttpContext?.User

				.FindFirst(ClaimTypes.NameIdentifier)

				?? _httpContextAccessor.HttpContext?.User

				.FindFirst("sub");

			if (claim == null) return 0;

			return int.Parse(claim.Value);

		}

		public string GetUserRole()

		{

			var claim = _httpContextAccessor.HttpContext?.User

				.FindFirst(ClaimTypes.Role);

			return claim?.Value ?? string.Empty;

		}

	}

}
