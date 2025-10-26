using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs.Clientes
{
    public class RegisterClienteRequest
    {
        public int? cliente_id { get; set; }
        public string? primer_nombre { get; set; }
        public string? segundo_nombre { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? identificacion { get; set; }
        public string? username { get; set; }

        public int? estado { get; set; }
    }
}
