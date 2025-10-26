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
    public class CrearCuentaAhorroPlanCommandHandler : BaseCommand, IDecoradorRequestHandler<CrearCuentaAhorroPlanCommand, ResponseBase<CrearCuentaAhorroPlanResponse>>
    {
        private readonly IClientesService _clientesService;
        private readonly ICuentasService _cuentasService;

        public CrearCuentaAhorroPlanCommandHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService, ICuentasService cuentasService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<CrearCuentaAhorroPlanResponse>> Handle(CrearCuentaAhorroPlanCommand request, CancellationToken cancellationToken)
        {
            CrearCuentaAhorroPlanRequest RequestData = request.Request!;
            Guid IdTraking = (Guid)request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            CrearCuentaAhorroPlanResponse objResponse = new CrearCuentaAhorroPlanResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorId((int)RequestData.cliente_id!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                ResponseBase<GetCuentaResponse> objCuenta = await _cuentasService.CuentaPorId((int)RequestData.cuenta_id!, IdTraking);
                if (!objCuenta.error.success)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                clienteDto cliente = objCliente.data!.cliente!.FirstOrDefault()!;
                cuentaDto cuenta = objCuenta.data!.cuenta!.FirstOrDefault()!;

                if (cuenta.saldo_disponible < 100)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, (int)TypeError.SaldoNoDisponible, Status: 500);

                if (RequestData.monto < 100)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, (int)TypeError.SaldoNoDisponible, Status: 500);

                ResponseBase<CrearNumeroCuentaResponse> objNumeroCuenta = await _cuentasService.GenerarNumeroCuenta(new DTOs.Cuentas.CrearNumeroCuentaRequest()
                {
                    agencia_id = 1,
                    banco = "666",
                    producto_id = 3!,
                }, IdTraking);

                if (!objNumeroCuenta.error.success)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                string numero_cuenta = objNumeroCuenta.data!.numero_cuenta!;

                RegisterCuentaRequest cuentaPlan = new RegisterCuentaRequest()
                {
                    agencia_id = 1,
                    cliente_id = cliente.cliente_id,
                    fecha_apertura = DateTime.Now,
                    fecha_ultima_transaccion = DateTime.Now,
                    numero_cuenta = numero_cuenta,
                    producto_id = 3,
                    moneda = RequestData.moneda,
                    saldo_actual = 0,
                    saldo_disponible = 0,
                    tasa_interes = 0.03M,
                    tipo_cuenta = "AHO",
                    usuario_creacion = "web",
                    estado = 1
                    
                };

                ResponseBase<RegisterCuentaResponse> objCuentaPlan = await _cuentasService.RegisterCuenta(cuentaPlan, IdTraking);
                if (!objCuenta.error.success)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, objCliente.error.codeError, Status: 500);


                cuentaDto objCuentaPlanReg = objCuentaPlan.data!.cuenta!;
                //Movimiento Debito
                await _cuentasService.RegistrarMovimiento(new RegisterMovimientoRequest()
                {
                    cuenta_id = cuenta.cuenta_id,
                    estado_movimiento = "Completo",
                    estado = 1,
                    fecha_hora = DateTime.Now,
                    monto = -RequestData.monto,
                    motivo = "Transferencia apertura Ahorro Plan",
                    naturaleza = "DEB",
                    referencia = "",
                    saldo_resultante = cuenta.saldo_disponible - RequestData.monto,
                    tipo_movimiento = "Transferencia"
                }, IdTraking);

                await _cuentasService.RegisterCuenta(new RegisterCuentaRequest()
                {
                    fecha_cierre = cuenta.fecha_cierre,
                    fecha_ultima_transaccion = DateTime.Now,
                    saldo_actual = cuenta.saldo_actual - RequestData.monto,
                    cuenta_id = cuenta.cuenta_id,
                    saldo_disponible = cuenta.saldo_disponible - RequestData.monto,
                    tasa_interes = cuenta.tasa_interes,
                    estado = (int)TypeStatus.Active
                }, IdTraking);

                //Movimiento Credito
                await _cuentasService.RegistrarMovimiento(new RegisterMovimientoRequest()
                {
                    cuenta_id = objCuentaPlanReg.cuenta_id,
                    estado_movimiento = "Completo",
                    estado = 1,
                    fecha_hora = DateTime.Now,
                    monto = RequestData.monto,
                    motivo = "Transferencia apertura Ahorro Plan",
                    naturaleza = "CRE",
                    referencia = "",
                    saldo_resultante = objCuentaPlanReg.saldo_disponible + RequestData.monto,
                    tipo_movimiento = "Transferencia"
                }, IdTraking);
             
                await _cuentasService.RegisterCuenta(new RegisterCuentaRequest()
                {
                    fecha_cierre = objCuentaPlanReg.fecha_cierre,
                    fecha_ultima_transaccion = DateTime.Now,
                    saldo_actual = objCuentaPlanReg.saldo_actual + RequestData.monto,
                    cuenta_id = objCuentaPlanReg.cuenta_id,
                    saldo_disponible = objCuentaPlanReg.saldo_disponible + RequestData.monto,
                    tasa_interes = objCuentaPlanReg.tasa_interes,
                    estado = (int)TypeStatus.Active
                }, IdTraking);

                ResponseBase<GetCuentaResponse> objCuentaPlanFin = await _cuentasService.CuentaPorId((int)objCuentaPlanReg.cuenta_id!, IdTraking);
                if (!objCuentaPlanFin.error.success)
                    return await ErrorResponse<CrearCuentaAhorroPlanResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cuenta = objCuentaPlan.data!.cuenta!;
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<CrearCuentaAhorroPlanResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
