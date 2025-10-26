using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs.Cuentas
{
    public class GetMovimientoResponse
    {
        public List<movimientoDto>? movimiento { get; set; }
    }
}
