using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs.Cuentas
{
    public class CrearNumeroCuentaRequest
    {
        public int? agencia_id { get; set; }
        public string? banco { get; set; } //123
        public int? producto_id { get; set; } //1
    }
}
