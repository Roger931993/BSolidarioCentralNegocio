using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using CentralNegocio.Application.Features.Commands;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.Features.Handlers
{
    public class CrearCuentaCommandHandler : BaseCommand, IDecoradorRequestHandler<CrearCuentaCommand, ResponseBase<CrearCuentaResponse>>
    {
        private readonly IClientesService _clientesService;
        private readonly ICuentasService _cuentasService;

        public CrearCuentaCommandHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService, ICuentasService cuentasService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<CrearCuentaResponse>> Handle(CrearCuentaCommand request, CancellationToken cancellationToken)
        {
            CrearCuentaRequest RequestData = request.Request!;
            Guid IdTraking = (Guid)request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            CrearCuentaResponse objResponse = new CrearCuentaResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorId((int)RequestData.cliente_id!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<CrearCuentaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                clienteDto cliente = objCliente.data!.cliente!.FirstOrDefault()!;

                ResponseBase<CrearNumeroCuentaResponse> objNumeroCuenta = await _cuentasService.GenerarNumeroCuenta(new DTOs.Cuentas.CrearNumeroCuentaRequest()
                {
                    agencia_id = 1,
                    banco = "666",
                    producto_id = RequestData.producto_id!,
                } , IdTraking);

                if (!objNumeroCuenta.error.success)
                    return await ErrorResponse<CrearCuentaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                string numero_cuenta = objNumeroCuenta.data!.numero_cuenta!;

                RegisterCuentaRequest cuenta = new RegisterCuentaRequest()
                {
                    agencia_id = 1,
                    cliente_id = cliente.cliente_id,
                    fecha_apertura = DateTime.Now,
                    fecha_ultima_transaccion = DateTime.Now,
                    numero_cuenta = numero_cuenta,
                    producto_id = RequestData.producto_id,
                    moneda = RequestData.moneda,
                    saldo_actual = 0,
                    saldo_disponible = 0,
                    tasa_interes = 0.01M,
                    tipo_cuenta = RequestData.tipo_cuenta,
                    usuario_creacion = "web",
                    estado = 1
                    
                };

                ResponseBase<RegisterCuentaResponse> objCuenta = await _cuentasService.RegisterCuenta(cuenta, IdTraking);
                if (!objCuenta.error.success)
                    return await ErrorResponse<CrearCuentaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cuenta = objCuenta.data!.cuenta!;

            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<CrearCuentaResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
