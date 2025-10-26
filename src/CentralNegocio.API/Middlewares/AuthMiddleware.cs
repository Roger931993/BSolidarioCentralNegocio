using CentralNegocio.Application.DTOs.Auth;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using Microsoft.Extensions.Primitives;

namespace CentralNegocio.API.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IServiceScopeFactory _scopeFactory;        
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IMemoryCacheLocalService memoryCacheLocalService, IServiceScopeFactory scopeFactory, IConfiguration configuration, IAuthService authService)
        {
            _next = next;
            _logger = logger;
            _memoryCacheLocalService = memoryCacheLocalService;
            this._scopeFactory = scopeFactory;            
            this._configuration = configuration;
            this._authService = authService;
        }

        public async Task Invoke(HttpContext context)
        {                  
            try
            {
                IHeaderDictionary headers = context.Request.Headers;
                Guid id = Guid.NewGuid();
                if (!headers.Any(x => x.Key == "IdTraker"))
                    context.Request.Headers.Add("IdTraker", id.ToString());

                string idTraker = string.Empty;
                if (headers.TryGetValue("IdTraker", out StringValues values))
                {
                    idTraker = values.FirstOrDefault()!.ToString();
                    id = Guid.Parse(idTraker);
                }


                #region Valido Autenticacion
                AuthResponse objresponse = await _memoryCacheLocalService.GetCachedData<AuthResponse>("Token");
                if (objresponse != null)
                {
                    DateTime fecha_expiracion = objresponse.creation_date!.Value.AddSeconds(objresponse.expires_in!.Value);
                    if (DateTime.Now >= fecha_expiracion)
                    {
                        //expiro_token -> solicitar 1
                        await _memoryCacheLocalService.SetCachedData(id.ToString(), new DataCacheLocal());
                        AuthResponse response = await _authService.GetToken(id);
                        if (response != null)
                        {
                            await _memoryCacheLocalService.SetCachedData("Token", response);
                        }
                    }
                }
                else
                {
                    await _memoryCacheLocalService.SetCachedData(id.ToString(), new DataCacheLocal());
                    AuthResponse response = await _authService.GetToken(id);
                    if (response != null)
                    {
                        await _memoryCacheLocalService.SetCachedData("Token", response);
                    }
                }                                                                
                #endregion                
                await _next(context);                                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en tareas concurrentes en middleware de Auth");             
            }
            finally
            {                                
            }
        }
    }
}
