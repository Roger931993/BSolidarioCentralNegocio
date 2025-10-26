using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.Features.Commands;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.Features.Handlers
{
    public class SessionClienteCommandHandler : BaseCommand, IDecoradorRequestHandler<SessionClienteCommand, ResponseBase<SessionClienteResponse>>
    {
        private readonly IClientesService _clientesService;

        public SessionClienteCommandHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
        }

        public async Task<ResponseBase<SessionClienteResponse>> Handle(SessionClienteCommand request, CancellationToken cancellationToken)
        {
            SessionClienteRequest RequestData = request.Request!;
            Guid IdTraking = (Guid)request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            SessionClienteResponse objResponse = new SessionClienteResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorUserName(RequestData.usuario!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<SessionClienteResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                if (!objCliente.data!.cliente!.Any())                
                    return await ErrorResponse<SessionClienteResponse>(IdTraking, (int)TypeError.NoData, Status: 500);

                //Validar Contraseña de cliente

                objResponse.session_id = Guid.NewGuid();

                await _redisCache.SetAsync(objResponse.session_id.ToString()!, objCliente.data.cliente);
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<SessionClienteResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
