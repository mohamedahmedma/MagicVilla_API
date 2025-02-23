using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using MagicVilla_Web.Models;
using MagicVilla_Utility;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _ClientFactory;
        private string villaUrl;
        public VillaService(IHttpClientFactory clientFactory , IConfiguration configuration) : base(clientFactory)
        {
            _ClientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> CreateAsync<T>(VillaCreateDTO entity, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = entity,
                Url = villaUrl + "/api/v1/VillaAPI",
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaUrl + "/api/v1/VillaAPI/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>( string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "/api/v1/VillaAPI",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "/api/v1/VillaAPI/" + id,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(VillaUpdateDTO entity, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = entity,
                Url = villaUrl + "/api/v1/VillaAPI/" + entity.Id,
                Token = token
            });
        }
    }
}
