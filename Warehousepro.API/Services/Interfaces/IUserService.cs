using WarehousePro.API.DTOs.User;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IUserService
	{
		Task<List<UserResponseDto>> GetAllAsync();
		Task<UserResponseDto?> GetByIdAsync(int id);
		Task<UserResponseDto> CreateAsync(UserCreateDto dto);
		Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto);
		Task<bool> DeleteAsync(int id);
		Task<bool> EmailExistsAsync(string email);
	}
}