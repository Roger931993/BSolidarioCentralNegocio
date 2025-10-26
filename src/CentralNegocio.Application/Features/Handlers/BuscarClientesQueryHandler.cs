using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.Features.Handlers
{
    public class BuscarClientesQueryHandler : BaseCommand, IDecoradorRequestHandler<BuscarClientesQuery, ResponseBase<BuscarClientesResponse>>
    {
        private readonly IClientesService _clientesService;
        private readonly ICuentasService _cuentasService;
        public BuscarClientesQueryHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService, ICuentasService cuentasService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<BuscarClientesResponse>> Handle(BuscarClientesQuery request, CancellationToken cancellationToken)
        {
            BuscarClientesRequest RequestData = request.request.Request!;
            Guid IdTraking = (Guid)request.request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            BuscarClientesResponse objResponse = new BuscarClientesResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.Clientes(IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<BuscarClientesResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.clientes = objCliente.data!.cliente;              
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<BuscarClientesResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
