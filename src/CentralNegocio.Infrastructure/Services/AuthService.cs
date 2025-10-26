using CentralNegocio.Application.DTOs.Auth;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using CentralNegocio.Infrastructure.Services.Common;
using CentralNegocio.Infrastructure.Services.Common.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace CentralNegocio.Infrastructure.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IConfiguration _configuration;
        private readonly IApiUrl _apiUrl;
        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCacheLocalService memoryCacheLocalService, ApiConnectionDto apiConnectionDto, IConfiguration configuration)
        {
            this._httpClient = httpClient;
            this._httpContextAccessor = httpContextAccessor;
            this._memoryCacheLocalService = memoryCacheLocalService;
            this._configuration = configuration;
            this._apiUrl = apiConnectionDto.Values!["Auth"];
        }

        public async Task<AuthResponse> GetToken(Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                string client_id = _configuration["Settings:ClientId"]!;
                string client_secret = _configuration["Settings:ClientSecret"]!;
                string grant_type = _configuration["Settings:grant_type"]!;
                string scope = _configuration["Settings:scope"]!;

                Dictionary<string,string> credentials = new Dictionary<string, string>
                {
                    { "client_id", client_id },
                    { "client_secret", client_secret },
                    { "grant_type", grant_type },
                    { "scope", scope }
                };

                var content = new FormUrlEncodedContent(credentials);                

                var response = await _httpClient.PostAsync($"{_apiUrl.Url}/api/token", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseJson2 = await response.Content.ReadAsStringAsync();
                    await AddLogOutput(responseJson2, 200, cachelocal);
                    AuthResponse resultService1 = JsonSerializer.Deserialize<AuthResponse>(responseJson2, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;

                    return resultService1;
                }

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                await AddLogOutput(responseJson, 200, cachelocal);
                JToken jToken = JObject.Parse(responseJson);
                string access_token = jToken["access_token"]!.ToString();
                string token_type = jToken["token_type"]!.ToString();
                string expires_in = jToken["expires_in"]!.ToString();
                string creation_date = jToken["creation_date"]!.ToString();


                AuthResponse resultService = new AuthResponse()
                {
                    access_token = access_token,
                    token_type = token_type,
                    creation_date = DateTime.Parse(creation_date),
                    expires_in = int.Parse(expires_in)
                };

                return resultService;
            }
            catch (Exception ex)
            {
                await AddLogError(string.Empty, 500, ex, cachelocal);
                return null;
            }
            finally
            {
                await AddLogInput(string.Empty, 200, cachelocal);
            }
        }
    }
}
