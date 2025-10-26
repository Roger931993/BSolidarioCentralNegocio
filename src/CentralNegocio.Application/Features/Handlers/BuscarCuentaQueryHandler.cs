using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.Features.Handlers
{
    public class BuscarCuentaQueryHandler : BaseCommand, IDecoradorRequestHandler<BuscarCuentaQuery, ResponseBase<BuscarCuentaResponse>>
    {
        private readonly ICuentasService _cuentasService;

        public BuscarCuentaQueryHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, ICuentasService cuentasService ) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<BuscarCuentaResponse>> Handle(BuscarCuentaQuery request, CancellationToken cancellationToken)
        {
            BuscarCuentaRequest RequestData = request.request.Request!;
            Guid IdTraking = (Guid)request.request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            BuscarCuentaResponse objResponse = new BuscarCuentaResponse();
            try
            {
                ResponseBase<GetCuentaResponse> objCliente = await _cuentasService.CuentaPorId((int)RequestData.cuenta_id!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<BuscarCuentaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cuentas = objCliente.data!.cuenta!.FirstOrDefault();                
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<BuscarCuentaResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
