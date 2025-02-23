using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using LoginRequestDTO = MagicVilla_Web.Models.DTO.LoginRequestDTO;
using RegisterationRequestDTO = MagicVilla_Web.Models.DTO.RegisterationRequestDTO;
namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _ClientFactory;
        private string villaUrl = "";
        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _ClientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaNumberAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = villaUrl + "/api/v1/UsersAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = villaUrl + "/api/v1/UsersAuth/register"
            });
        }
    }
}
