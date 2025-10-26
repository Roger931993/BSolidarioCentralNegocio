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
    public class CrearDepositoCommandHandler : BaseCommand, IDecoradorRequestHandler<CrearDepositoCommand, ResponseBase<CrearDepositoResponse>>
    {
        private readonly ICuentasService _cuentasService;
        private readonly IClientesService _clientesService;

        public CrearDepositoCommandHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, ICuentasService cuentasService , IClientesService clientesService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._cuentasService = cuentasService;
            this._clientesService = clientesService;
        }

        public async Task<ResponseBase<CrearDepositoResponse>> Handle(CrearDepositoCommand request, CancellationToken cancellationToken)
        {
            CrearDepositoRequest RequestData = request.Request!;
            Guid IdTraking = (Guid)request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            CrearDepositoResponse objResponse = new CrearDepositoResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorId((int)RequestData.cliente_id!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<CrearDepositoResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                ResponseBase<GetCuentaResponse> objCuenta = await _cuentasService.CuentaPorId((int)RequestData.cuenta_id!, IdTraking);
                if (!objCuenta.error.success)
                    return await ErrorResponse<CrearDepositoResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                clienteDto cliente = objCliente.data!.cliente!.FirstOrDefault()!;
                cuentaDto cuenta = objCuenta.data!.cuenta!.FirstOrDefault()!;              
                        

                //Movimiento Credito
                await _cuentasService.RegistrarMovimiento(new RegisterMovimientoRequest()
                {
                    cuenta_id = cuenta.cuenta_id,
                    estado_movimiento = "Completo",
                    estado = 1,
                    fecha_hora = DateTime.Now,
                    monto = RequestData.monto,
                    motivo = "Deposito cuenta",
                    naturaleza = "CRE",
                    referencia = "",
                    saldo_resultante = cuenta.saldo_disponible + RequestData.monto,
                    tipo_movimiento = "Transferencia"
                }, IdTraking);

                await _cuentasService.RegisterCuenta(new RegisterCuentaRequest()
                {
                    fecha_cierre = cuenta.fecha_cierre,
                    fecha_ultima_transaccion = DateTime.Now,
                    saldo_actual = cuenta.saldo_actual + RequestData.monto,
                    cuenta_id = cuenta.cuenta_id,
                    saldo_disponible = cuenta.saldo_disponible + RequestData.monto,
                    tasa_interes = cuenta.tasa_interes,
                    estado = (int)TypeStatus.Active
                }, IdTraking);

                ResponseBase<GetCuentaResponse> objCuentaDeposito = await _cuentasService.CuentaPorId((int)cuenta.cuenta_id!, IdTraking);
                if (!objCuentaDeposito.error.success)
                    return await ErrorResponse<CrearDepositoResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cuenta = objCuentaDeposito.data!.cuenta!.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<CrearDepositoResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
