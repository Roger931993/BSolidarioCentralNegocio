using AutoMapper;
using CentralNegocio.Application.DTOs;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using CentralNegocio.Shared.Helpers;
using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.Features.Handlers
{
    public class ProyeccionAhorroQueryHandler : BaseCommand, IDecoradorRequestHandler<ProyeccionAhorroQuery, ResponseBase<ProyeccionAhorroResponse>>
    {
        private readonly IClientesService _clientesService;
        private readonly ICuentasService _cuentasService;

        public ProyeccionAhorroQueryHandler(IErrorCatalogException errorCatalogException, IRedisCache redisCache, IMemoryCacheLocalService memoryCacheLocalService, IMapper mapper, IClientesService clientesService, ICuentasService cuentasService) : base(errorCatalogException, redisCache, memoryCacheLocalService, mapper)
        {
            this._clientesService = clientesService;
            this._cuentasService = cuentasService;
        }

        public async Task<ResponseBase<ProyeccionAhorroResponse>> Handle(ProyeccionAhorroQuery request, CancellationToken cancellationToken)
        {
            ProyeccionAhorroRequest RequestData = request.request.Request!;
            Guid IdTraking = (Guid)request.request.IdTraking!;
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            ProyeccionAhorroResponse objResponse = new ProyeccionAhorroResponse();
            try
            {
                ResponseBase<GetClienteResponse> objCliente = await _clientesService.ClientePorId((int)RequestData.cliente_id!, IdTraking);
                if (!objCliente.error.success)
                    return await ErrorResponse<ProyeccionAhorroResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                if (!objCliente.data!.cliente!.Any())
                    return await ErrorResponse<ProyeccionAhorroResponse>(IdTraking, (int)TypeError.NoData, Status: 500);

                objResponse.cliente = objCliente.data!.cliente!.FirstOrDefault();

                ResponseBase<GetCuentaResponse> objCuentas = await _cuentasService.CuentaPorId((int)RequestData.cuenta_id!, IdTraking);
                if (!objCuentas.error.success)
                    return await ErrorResponse<ProyeccionAhorroResponse>(IdTraking, objCliente.error.codeError, Status: 500);

                objResponse.cliente = objCliente.data.cliente!.FirstOrDefault();
                objResponse.cuenta = objCuentas.data!.cuenta!.FirstOrDefault();

                if (objResponse.cuenta != null)
                {
                    //Calculo de Proyeccion
                    List<proyeccionDto> objproyeccion = new List<proyeccionDto>();
                    decimal meses = 12;
                    DateTime dateTime = DateTime.Now;
                    int mes_inicial = dateTime.Month;
                    int año_inicial = dateTime.Year;
                    decimal? saldo_inicial = objResponse.cuenta!.saldo_disponible.ToNumDecimal(2);
                    decimal? interes_mensual = objResponse.cuenta.tasa_interes / meses;
                    int count = 0;
                    for (int i = 0; i < meses; i++)
                    {                        
                        int mes = mes_inicial + count;
                        count++;
                        int año = año_inicial;
                        if (mes > 12)
                        {
                            año_inicial++;
                            mes_inicial = 1;
                            mes = mes_inicial;
                            count = 1;
                            año = año_inicial;
                        }

                        decimal? valor_interes = saldo_inicial * interes_mensual;
                        saldo_inicial += valor_interes;

                        proyeccionDto proyeccion_mes = new proyeccionDto()
                        {
                            id = i + 1,
                            mes = mes,
                            año = año,
                            interes = objResponse.cuenta!.tasa_interes,
                            monto_interes = valor_interes,
                            monto_total = saldo_inicial
                        };
                        objproyeccion.Add(proyeccion_mes);
                    }
                    objResponse.total = saldo_inicial.ToNumDecimal(2);
                    objResponse.proyeccion = objproyeccion;
                }
            }
            catch (Exception ex)
            {
                await AddLogError(RequestData, 500, ex, cachelocal);
                return await ErrorResponseEx<ProyeccionAhorroResponse>(IdTraking, ex, (int)TypeError.InternalError, Status: 500);
            }
            return await OkResponse(objResponse);
        }
    }
}
