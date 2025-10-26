using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs.Clientes
{
    public class GetClienteResponse
    {
        public List<clienteDto>? cliente { get; set; }
    }
}
