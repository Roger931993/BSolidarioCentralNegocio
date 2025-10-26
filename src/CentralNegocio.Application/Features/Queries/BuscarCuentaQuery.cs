using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.Features.Queries
{
    public record BuscarCuentaQuery(RequestBase<BuscarCuentaRequest> request) : IRequest<ResponseBase<BuscarCuentaResponse>>;

    public class BuscarCuentaRequest
    {
        public int? cuenta_id { get; set; }
    }

    public class BuscarCuentaResponse
    {        
        public cuentaDto? cuentas { get; set; }
    }
}
