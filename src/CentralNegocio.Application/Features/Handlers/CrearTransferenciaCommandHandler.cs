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
    internal class CrearTransferenciaCommandHandler : BaseCommand, IDecoradorRequestHandler<CrearTransferenciaCommand, ResponseBase<CrearTransferenciaResponse>>
    {
        private readonly IClientesService _clientesService;
        private readonly ICuentasService _cuentasService;

        public CrearTransferenciaCommandHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService, ICuentasService cuentasService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<CrearTransferenciaResponse>> Handle(CrearTransferenciaCommand request, CancellationToken cancellationToken)
        {
            CrearTransferenciaRequest RequestData = request.Request!;
            Guid IdTraking = (Guid)request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            CrearTransferenciaResponse objResponse = new CrearTransferenciaResponse();
            try
            {
                //Origen
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorId((int)RequestData.cliente_id_origen!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<CrearTransferenciaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                ResponseBase<GetCuentaResponse> objCuenta = await _cuentasService.CuentaPorId((int)RequestData.cuenta_id_origen!, IdTraking);
                if (!objCuenta.error.success)
                    return await ErrorResponse<CrearTransferenciaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                clienteDto cliente_origen = objCliente.data!.cliente!.FirstOrDefault()!;
                cuentaDto cuenta_origen = objCuenta.data!.cuenta!.FirstOrDefault()!;

                //Origen
                ResponseBase<GetClienteResponse> objCliente_Destino = await _clientesService.ClientePorId((int)RequestData.cliente_id_destino!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<CrearTransferenciaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                ResponseBase<GetCuentaResponse> objCuenta_Destino = await _cuentasService.CuentaPorId((int)RequestData.cuenta_id_destino!, IdTraking);
                if (!objCuenta.error.success)
                    return await ErrorResponse<CrearTransferenciaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                clienteDto cliente_destino = objCliente.data!.cliente!.FirstOrDefault()!;
                cuentaDto cuenta_destino = objCuenta.data!.cuenta!.FirstOrDefault()!;

                //Movimiento Debito
                await _cuentasService.RegistrarMovimiento(new RegisterMovimientoRequest()
                {
                    cuenta_id = cuenta_origen.cuenta_id,
                    estado_movimiento = "Completo",
                    estado = 1,
                    fecha_hora = DateTime.Now,
                    monto = -RequestData.monto,
                    motivo = RequestData.motivo,
                    naturaleza = "DEB",
                    referencia = "",
                    saldo_resultante = cuenta_origen.saldo_disponible - RequestData.monto,
                    tipo_movimiento = "Transferencia"
                }, IdTraking);

                await _cuentasService.RegisterCuenta(new RegisterCuentaRequest()
                {
                    fecha_cierre = cuenta_origen.fecha_cierre,
                    fecha_ultima_transaccion = DateTime.Now,
                    saldo_actual = cuenta_origen.saldo_actual - RequestData.monto,
                    cuenta_id = cuenta_origen.cuenta_id,
                    saldo_disponible = cuenta_origen.saldo_disponible - RequestData.monto,
                    tasa_interes = cuenta_origen.tasa_interes,
                    estado = (int)TypeStatus.Active
                }, IdTraking);


                //Movimiento Credito
                await _cuentasService.RegistrarMovimiento(new RegisterMovimientoRequest()
                {
                    cuenta_id = cuenta_destino.cuenta_id,
                    estado_movimiento = "Completo",
                    estado = 1,
                    fecha_hora = DateTime.Now,
                    monto = RequestData.monto,
                    motivo = RequestData.motivo,
                    naturaleza = "CRE",
                    referencia = "",
                    saldo_resultante = cuenta_destino.saldo_disponible + RequestData.monto,
                    tipo_movimiento = "Transferencia"
                }, IdTraking);

                await _cuentasService.RegisterCuenta(new RegisterCuentaRequest()
                {
                    fecha_cierre = cuenta_destino.fecha_cierre,
                    fecha_ultima_transaccion = DateTime.Now,
                    saldo_actual = cuenta_destino.saldo_actual + RequestData.monto,
                    cuenta_id = cuenta_destino.cuenta_id,
                    saldo_disponible = cuenta_destino.saldo_disponible + RequestData.monto,
                    tasa_interes = cuenta_destino.tasa_interes,
                    estado = (int)TypeStatus.Active
                }, IdTraking);

                ResponseBase<GetCuentaResponse> objCuentaTran_origen = await _cuentasService.CuentaPorId((int)cuenta_origen.cuenta_id!, IdTraking);
                if (!objCuentaTran_origen.error.success)
                    return await ErrorResponse<CrearTransferenciaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                ResponseBase<GetCuentaResponse> objCuentaTran_destino = await _cuentasService.CuentaPorId((int)cuenta_destino.cuenta_id!, IdTraking);
                if (!objCuentaTran_destino.error.success)
                    return await ErrorResponse<CrearTransferenciaResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cuenta_origen = objCuentaTran_origen.data!.cuenta!.FirstOrDefault();
                objResponse.cuenta_destino = objCuentaTran_destino.data!.cuenta!.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<CrearTransferenciaResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
