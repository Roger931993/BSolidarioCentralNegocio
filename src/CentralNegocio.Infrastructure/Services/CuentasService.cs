using CentralNegocio.Application.DTOs.Auth;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
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
    public class CuentasService: BaseService, ICuentasService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IApiUrl _apiUrl;
        public CuentasService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCacheLocalService memoryCacheLocalService, ApiConnectionDto apiConnectionDto)
        {
            this._httpClient = httpClient;
            this._httpContextAccessor = httpContextAccessor;
            this._memoryCacheLocalService = memoryCacheLocalService;
            this._apiUrl = apiConnectionDto.Values!["Cuentas"];
        }
        public async Task<ResponseBase<GetCuentaResponse>> CuentaPorId(int cuenta_id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Cuentas/id/{cuenta_id}");
                ResponseBase<GetCuentaResponse> resultService = await Util.ConvertResponse<ResponseBase<GetCuentaResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(cuenta_id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetCuentaResponse>
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
                await AddLogInput(cuenta_id.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<GetCuentaResponse>> CuentaPorClienteId(int cliente_id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Cuentas/cliente/{cliente_id}");
                ResponseBase<GetCuentaResponse> resultService = await Util.ConvertResponse<ResponseBase<GetCuentaResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(cliente_id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetCuentaResponse>
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

        public async Task<ResponseBase<RegisterCuentaResponse>> RegisterCuenta(RegisterCuentaRequest request, Guid IdTraker)
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

                var response = await _httpClient.PostAsync($"{_apiUrl.Url}/api/Cuentas/registrar", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseJson2 = await response.Content.ReadAsStringAsync();
                    await AddLogOutput(responseJson2, 200, cachelocal);
                    ResponseBase<RegisterCuentaResponse> resultService1 = JsonSerializer.Deserialize<ResponseBase<RegisterCuentaResponse>>(responseJson2, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;

                    return resultService1;
                }

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                await AddLogOutput(responseJson, 200, cachelocal);
                ResponseBase<RegisterCuentaResponse> resultService = JsonSerializer.Deserialize<ResponseBase<RegisterCuentaResponse>>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                return resultService;
            }
            catch (Exception ex)
            {
                await AddLogError(request, 500, ex, cachelocal);
                return new ResponseBase<RegisterCuentaResponse>
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

        public async Task<ResponseBase<CrearNumeroCuentaResponse>> GenerarNumeroCuenta(CrearNumeroCuentaRequest request, Guid IdTraker)
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

                var response = await _httpClient.PostAsync($"{_apiUrl.Url}/api/Cuentas/generar-numero-cuenta", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseJson2 = await response.Content.ReadAsStringAsync();
                    await AddLogOutput(responseJson2, 200, cachelocal);
                    ResponseBase<CrearNumeroCuentaResponse> resultService1 = JsonSerializer.Deserialize<ResponseBase<CrearNumeroCuentaResponse>>(responseJson2, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;

                    return resultService1;
                }

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                await AddLogOutput(responseJson, 200, cachelocal);
                ResponseBase<CrearNumeroCuentaResponse> resultService = JsonSerializer.Deserialize<ResponseBase<CrearNumeroCuentaResponse>>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                return resultService;
            }
            catch (Exception ex)
            {
                await AddLogError(request, 500, ex, cachelocal);
                return new ResponseBase<CrearNumeroCuentaResponse>
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

        public async Task<ResponseBase<GetMovimientoResponse>> MovimientoPorId(int movimiento_id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Movimientos/id/{movimiento_id}");
                ResponseBase<GetMovimientoResponse> resultService = await Util.ConvertResponse<ResponseBase<GetMovimientoResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(movimiento_id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetMovimientoResponse>
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
                await AddLogInput(movimiento_id.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<GetMovimientoResponse>> MovimientoPorCuentaId(int cuenta_id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Movimientos/cuenta/{cuenta_id}");
                ResponseBase<GetMovimientoResponse> resultService = await Util.ConvertResponse<ResponseBase<GetMovimientoResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(cuenta_id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetMovimientoResponse>
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
                await AddLogInput(cuenta_id.ToString(), 200, cachelocal);
            }
        }

        public async Task<ResponseBase<GetMovimientoResponse>> MovimientoPorClienteId(int cliente_id, Guid IdTraker)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraker.ToString());
            try
            {
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                var token = objresponse.access_token;
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.GetAsync($"{_apiUrl.Url}/api/Movimientos/cliente/{cliente_id}");
                ResponseBase<GetMovimientoResponse> resultService = await Util.ConvertResponse<ResponseBase<GetMovimientoResponse>>(response);
                return resultService!;
            }
            catch (Exception ex)
            {
                await AddLogError(cliente_id.ToString(), 500, ex, cachelocal);
                return new ResponseBase<GetMovimientoResponse>
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

        public async Task<ResponseBase<RegisterMovimientoResponse>> RegistrarMovimiento(RegisterMovimientoRequest request, Guid IdTraker)
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

                var response = await _httpClient.PostAsync($"{_apiUrl.Url}/api/Movimientos/registrar", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseJson2 = await response.Content.ReadAsStringAsync();
                    await AddLogOutput(responseJson2, 200, cachelocal);
                    ResponseBase<RegisterMovimientoResponse> resultService1 = JsonSerializer.Deserialize<ResponseBase<RegisterMovimientoResponse>>(responseJson2, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;

                    return resultService1;
                }

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                await AddLogOutput(responseJson, 200, cachelocal);
                ResponseBase<RegisterMovimientoResponse> resultService = JsonSerializer.Deserialize<ResponseBase<RegisterMovimientoResponse>>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                return resultService;
            }
            catch (Exception ex)
            {
                await AddLogError(request, 500, ex, cachelocal);
                return new ResponseBase<RegisterMovimientoResponse>
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
