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
    public class CrearClienteCommandHandler : BaseCommand, IDecoradorRequestHandler<CrearClienteCommand, ResponseBase<CrearClienteResponse>>
    {
        private readonly IClientesService _clientesService;

        public CrearClienteCommandHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
        }

        public async Task<ResponseBase<CrearClienteResponse>> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
        {
            CrearClienteRequest RequestData = request.Request!;
            Guid IdTraking = (Guid)request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            CrearClienteResponse objResponse = new CrearClienteResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorCedula(RequestData.identificacion!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<CrearClienteResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                GetClienteResponse dataCliente = objCliente.data!;
                ResponseBase<RegisterClienteResponse> registrado;
                if (dataCliente.cliente!.Any())
                {
                    registrado = await _clientesService.Register(new RegisterClienteRequest()
                    {
                        apellido_materno = dataCliente.cliente!.FirstOrDefault()!.apellido_materno,
                        apellido_paterno = dataCliente.cliente!.FirstOrDefault()!.apellido_paterno,
                        identificacion = dataCliente.cliente!.FirstOrDefault()!.identificacion,
                        primer_nombre = dataCliente.cliente!.FirstOrDefault()!.primer_nombre,
                        segundo_nombre = dataCliente.cliente!.FirstOrDefault()!.segundo_nombre,
                        cliente_id = dataCliente.cliente!.FirstOrDefault()!.cliente_id,
                    }, IdTraking);
                }
                else
                {
                    registrado = await _clientesService.Register(new RegisterClienteRequest()
                    {
                        apellido_materno = RequestData.apellido_materno,
                        apellido_paterno = RequestData.apellido_paterno,
                        identificacion = RequestData.identificacion,
                        primer_nombre = RequestData.primer_nombre,
                        segundo_nombre = RequestData.segundo_nombre,    
                        username = RequestData.user_name,                        
                    }, IdTraking);
                }
                if (!registrado.error.success)
                    return await ErrorResponse<CrearClienteResponse>(IdTraking, registrado.error.codeError, Status: 500);

                objResponse.cliente = registrado.data!.cliente;
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<CrearClienteResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
