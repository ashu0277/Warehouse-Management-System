using WarehousePro.API.DTOs.Auth;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IAuthService
	{
		Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
		Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
	}
}