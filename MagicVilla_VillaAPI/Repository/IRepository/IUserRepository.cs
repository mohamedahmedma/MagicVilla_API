using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRepository : IRepository<localUser>
    {
        bool IsUnigueUser(string username);
        Task<localUser>  UpdateAsync(localUser user);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginResponseDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
