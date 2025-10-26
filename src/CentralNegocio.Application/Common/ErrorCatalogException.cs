using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Catalog;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;

namespace CentralNegocio.Application.Common
{
    public class ErrorCatalogException : ApplicationException, IErrorCatalogException
    {
        private readonly IRedisCache _redisCache;
        private readonly ICatalogService _catalogService;

        public ErrorCatalogException(IRedisCache redisCache, ICatalogService catalogService)
        {
            this._redisCache = redisCache;
            this._catalogService = catalogService;
        }

        public int CodeError { get; private set; }
        public int StatusCodeError { get; private set; }
        public string? MessageError { get; private set; }
        public Guid? IdTraking { get; set; }
        public string Exception { get; set; }

        public async Task SetCodeError(int code, Guid? idTraking)
        {
            List<catalog_errorDto> objErrorCache = await _redisCache.GetAsync<List<catalog_errorDto>>($"catalog_error");
            objErrorCache = objErrorCache != null ? objErrorCache : new List<catalog_errorDto>();

            IdTraking = idTraking;
            catalog_errorDto objError = objErrorCache.FirstOrDefault(x => x.catalog_error_id == code)!;
            CodeError = code;
            if (objError != null)
            {
                MessageError = objError!.error_description;
                StatusCodeError = objError!.error_status_code;
            }
            else
            {
                ResponseBase<GetErrorByIdResponse> objError2 = await _catalogService.GetCatalogErrorById(code, (Guid)idTraking!);
                if (objError2.error.success)
                {
                    MessageError = objError2.data!.catalog_error!.error_description;
                    StatusCodeError = objError2.data!.catalog_error!.error_status_code;
                }
                else
                {
                    MessageError = "Error no encontrado en la base de datos";
                    StatusCodeError = 500;
                }
            }
        }

        public async Task SetCodeError(int code, Guid? idTraking, Exception ex)
        {
            List<catalog_errorDto> objErrorCache = await _redisCache.GetAsync<List<catalog_errorDto>>($"catalog_error");
            objErrorCache = objErrorCache != null ? objErrorCache : new List<catalog_errorDto>();

            IdTraking = idTraking;
            catalog_errorDto objError = objErrorCache.FirstOrDefault(x => x.catalog_error_id == code)!;
            CodeError = code;
            Exception = ex.Message;
            if (objError != null)
            {
                MessageError = objError!.error_description;
                StatusCodeError = objError!.error_status_code;                
            }
            else
            {
                ResponseBase<GetErrorByIdResponse> objError2 = await _catalogService.GetCatalogErrorById(code, (Guid)idTraking!);
                if (objError2.error.success)
                {
                    MessageError = objError2.data!.catalog_error!.error_description;
                    StatusCodeError = objError2.data!.catalog_error!.error_status_code;
                }
                else
                {
                    MessageError = "Error no encontrado en la base de datos";
                    StatusCodeError = 500;
                }
            }
        }

        public void SetCodeErrorMessage(int code, string? message, Guid? idTraking, int StatusCode = 500)
        {
            IdTraking = idTraking;
            CodeError = code;
            MessageError = message;
            StatusCodeError = StatusCode;
        }
        public void SetCodeErrorMessage(int code, Exception ex, string? message, Guid? idTraking, int StatusCode = 500)
        {
            IdTraking = idTraking;
            CodeError = code;
            MessageError = message;
            StatusCodeError = StatusCode;
            Exception = ex.Message;
        }
    }
}
