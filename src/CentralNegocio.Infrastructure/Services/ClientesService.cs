using CentralNegocio.Application.DTOs.Auth;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using CentralNegocio.Infrastructure.Services.Common;
using CentralNegocio.Infrastructure.Services.Common.Api;
using CentralNegocio.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Infrastructure.Services
{
    public class ClientesService: BaseService, IClientesService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IApiUrl _apiUrl;
        public ClientesService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCacheLocalService memoryCacheLocalService, ApiConnectionDto apiConnectionDto)
        {
            this._httpClient = httpClient;
            this._httpContextAccessor = httpContextAccessor;
            this._memoryCacheLocalService = memoryCacheLocalService;
            this._apiUrl = apiConnectionDto.Values!["Clientes"];
        }

        public async Task<ResponseBase<GetClienteResponse>> Clientes(Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Clientes");
                ResponseBase<GetClienteResponse> resultService = await Util.ConvertResponse<ResponseBase<GetClienteResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(string.Empty, 500, ex, cachelocal);
                return new ResponseBase<GetClienteResponse>
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
                await AddLogInput(string.Empty.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<GetClienteResponse>> ClientePorId(int cliente_id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Clientes/id/{cliente_id}");
                ResponseBase<GetClienteResponse> resultService = await Util.ConvertResponse<ResponseBase<GetClienteResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(cliente_id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetClienteResponse>
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
                await AddLogInput(cliente_id.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<GetClienteResponse>> ClientePorCedula(string identificacion, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Clientes/identificacion/{identificacion}");
                ResponseBase<GetClienteResponse> resultService = await Util.ConvertResponse<ResponseBase<GetClienteResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(identificacion.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetClienteResponse>
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
                await AddLogInput(identificacion.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<GetClienteResponse>> ClientePorUserName(string username, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Clientes/username/{username}");
                ResponseBase<GetClienteResponse> resultService = await Util.ConvertResponse<ResponseBase<GetClienteResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(username.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetClienteResponse>
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
                await AddLogInput(username.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<RegisterClienteResponse>> Register(RegisterClienteRequest request, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl.Url}/api/Clientes/registrar", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseJson2 = await response.Content.ReadAsStringAsync();
                    await AddLogOutput(responseJson2, (int)response.StatusCode, cachelocal);
                    if (!string.IsNullOrEmpty(responseJson2))
                    {
                        ResponseBase<RegisterClienteResponse> resultService1 = JsonSerializer.Deserialize<ResponseBase<RegisterClienteResponse>>(responseJson2, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })!;
                        return resultService1;
                    }
                    else
                    {
                        return new ResponseBase<RegisterClienteResponse>
                        {
                            error = new Error
                            {
                                codeError = (int)TypeError.InternalError,                                
                                success = false
                            }
                        };
                    }
                }

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                await AddLogOutput(responseJson, 200, cachelocal);
                ResponseBase<RegisterClienteResponse> resultService = JsonSerializer.Deserialize<ResponseBase<RegisterClienteResponse>>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                return resultService;
            }
            catch (Exception ex)
            {
                await AddLogError(request, 500, ex, cachelocal);
                return new ResponseBase<RegisterClienteResponse>
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
                await AddLogInput(request, 200, cachelocal);
            }
        }
    }
}
