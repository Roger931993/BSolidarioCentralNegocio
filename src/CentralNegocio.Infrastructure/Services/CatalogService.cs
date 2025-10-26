using CentralNegocio.Application.DTOs.Auth;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Catalog;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using CentralNegocio.Infrastructure.Services.Common;
using CentralNegocio.Infrastructure.Services.Common.Api;
using CentralNegocio.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Infrastructure.Services
{
    public class CatalogService : BaseService, ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IApiUrl _apiUrl;
        public CatalogService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCacheLocalService memoryCacheLocalService, ApiConnectionDto apiConnectionDto)
        {
            this._httpClient = httpClient;
            this._httpContextAccessor = httpContextAccessor;
            this._memoryCacheLocalService = memoryCacheLocalService;
            this._apiUrl = apiConnectionDto.Values!["Catalogos"];
        }

        public async Task<ResponseBase<GetErrorByIdResponse>> GetCatalogErrorById(int id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));          

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/code-error/{id}");               
                ResponseBase<GetErrorByIdResponse> resultService = await Util.ConvertResponse<ResponseBase<GetErrorByIdResponse>>(response);                     
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetErrorByIdResponse>
                {
                    error = new Error
                    {
                        codeError = (int)TypeError.InternalError,
                        messageError = ex.Message,
                        success = false
                    }
                };
            }
            finally
            {
                await AddLogInput(id.ToString(), 200, cachelocal);
            }
        }
    }
}
