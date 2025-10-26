using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using CentralNegocio.Shared.Constants;
using CentralNegocio.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CentralNegocio.API.Controllers
{
    public class CommonController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IRedisCache _redisCache;

        public CommonController(IMediator mediator, IMemoryCacheLocalService memoryCacheLocalService, IRedisCache redisCache)
        {
            _mediator = mediator;
            _memoryCacheLocalService = memoryCacheLocalService;
            this._redisCache = redisCache;
        }
        [NonAction]
        public async Task CreateDataCacheLocal<TRequest>(HttpContext context, TRequest command) where TRequest : IRequestBase
        {
            string idSession = context.User.Claims.FirstOrDefault(u => u.Type == Generals.Claims.idSession)?.Value!;
            string idUser = context.User.Claims.FirstOrDefault(u => u.Type == Generals.Claims.IdUser)?.Value!;
            string isAdmin = context.User.Claims.FirstOrDefault(u => u.Type == Generals.Claims.isAdmin)?.Value!;

            Endpoint endpoint = context.GetEndpoint() ?? throw new ArgumentException("Not find endpoints");
            ControllerActionDescriptor actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            DataCacheLocal dataCacheLocal = new DataCacheLocal();
            command.InfoSession = new InfoSessionDto()
            {
                SessionId = idSession,
                UserId = idUser.ToInteger(),
                IsAdmin = isAdmin == "true"
            };
            if (!command.IdTraking.HasValue)
            {
                TRequest reference = command;
                TRequest val = default!;
                if (val == null)
                {
                    val = reference;
                    reference = val;
                }

                reference.IdTraking = new Guid(context.Request.Headers["IdTraker"].ToString());
                dataCacheLocal.IdTracking = command.IdTraking;
                dataCacheLocal.RequestMethod = context.Request.Method;
                dataCacheLocal.RequestUrl = context.Request.Path;
                dataCacheLocal.CreatedAt = DateTime.UtcNow;
                await _memoryCacheLocalService.SetCachedData(dataCacheLocal.IdTracking.ToString()!, dataCacheLocal);
            }
        }

        [NonAction]
        public ObjectResult OkUrban<T>(ResponseBase<T> value)
        {
            if (value!.errorCatalogException != null)
            {
                object result = new
                {
                    error = new
                    {
                        value.errorCatalogException.CodeError,
                        value.errorCatalogException.MessageError,
                        Success = false,
                        MessageException = value.errorCatalogException.Exception ?? string.Empty
                    }
                };
                return StatusCode(value.errorCatalogException.StatusCodeError, result);
            }
            return Ok(new { value.data, value.error });
        }

        [NonAction]
        public ObjectResult OkUrbanPages<T>(ResponseBase<T> value)
        {
            if (value!.errorCatalogException != null)
            {
                object result = new
                {
                    error = new
                    {
                        value.errorCatalogException.CodeError,
                        value.errorCatalogException.MessageError,
                        Success = false,
                        MessageException = value.errorCatalogException.Exception ?? string.Empty
                    }
                };
                return StatusCode(value.errorCatalogException.StatusCodeError, result);
            }
            return Ok(new { value.data, value.error, value.pages });
        }
    }
}
