using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.Features.Handlers
{
    public class BuscarClienteQueryHandler : BaseCommand, IDecoradorRequestHandler<BuscarClienteQuery, ResponseBase<BuscarClienteResponse>>
    {
        private readonly IClientesService _clientesService;
        private readonly ICuentasService _cuentasService;

        public BuscarClienteQueryHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService, ICuentasService cuentasService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<BuscarClienteResponse>> Handle(BuscarClienteQuery request, CancellationToken cancellationToken)
        {
            BuscarClienteRequest RequestData = request.request.Request!;
            Guid IdTraking = (Guid)request.request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            BuscarClienteResponse objResponse = new BuscarClienteResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorCedula(RequestData.identificacion!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<BuscarClienteResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cliente = objCliente.data!.cliente!.FirstOrDefault();

                if (objResponse.cliente != null)
                {
                    ResponseBase<GetCuentaResponse> objCuentas = await _cuentasService.CuentaPorClienteId((int)objResponse.cliente!.cliente_id!, IdTraking);
                    if (!objCuentas.error.success)
                        return await ErrorResponse<BuscarClienteResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                    objResponse.cuentas = objCuentas.data!.cuenta;
                }
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<BuscarClienteResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
