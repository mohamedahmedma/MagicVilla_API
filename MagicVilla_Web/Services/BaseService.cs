using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using MagicVilla_Utility;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModle {  get; set; }
        public IHttpClientFactory httpclient {  get; set; }
        public BaseService(IHttpClientFactory httpclient)
        {
            responseModle = new();
            this.httpclient = httpclient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpclient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if(apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), encoding: System.Text.Encoding.UTF8 , "application/json");
                }
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiResponse = null;

                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer" , apiRequest.Token);
                }

                apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
                {
                    APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
					if (ApiResponse != null &&  ( ApiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest || ApiResponse.StatusCode == System.Net.HttpStatusCode.NotFound))
                    {
                        //ApiResponse.StatusCode = apiResponse.StatusCode;
                        ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        ApiResponse.IsSuccess = false;
						var res = JsonConvert.SerializeObject(ApiResponse);
						var returnobj = JsonConvert.DeserializeObject<T>(res);
                        return returnobj;
					}
				}
				catch (Exception ex)
                {
					var ApiResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return ApiResponse;
				}
                var Apiresp = JsonConvert.DeserializeObject<T>(apiContent);
				return Apiresp;
            }
            catch(Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}