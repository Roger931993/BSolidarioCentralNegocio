using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.Interfaces.Infraestructure
{
    public interface ICuentasService
    {
        Task<ResponseBase<GetCuentaResponse>> CuentaPorId(int cuenta_id, Guid IdTraker);
        Task<ResponseBase<GetCuentaResponse>> CuentaPorClienteId(int cliente_id, Guid IdTraker);
        Task<ResponseBase<RegisterCuentaResponse>> RegisterCuenta(RegisterCuentaRequest request, Guid IdTraker);
        Task<ResponseBase<CrearNumeroCuentaResponse>> GenerarNumeroCuenta(CrearNumeroCuentaRequest request, Guid IdTraker);
        Task<ResponseBase<GetMovimientoResponse>> MovimientoPorId(int movimiento_id, Guid IdTraker);
        Task<ResponseBase<GetMovimientoResponse>> MovimientoPorCuentaId(int cuenta_id, Guid IdTraker);
        Task<ResponseBase<GetMovimientoResponse>> MovimientoPorClienteId(int cliente_id, Guid IdTraker);
        Task<ResponseBase<RegisterMovimientoResponse>> RegistrarMovimiento(RegisterMovimientoRequest request, Guid IdTraker);
    }
}
