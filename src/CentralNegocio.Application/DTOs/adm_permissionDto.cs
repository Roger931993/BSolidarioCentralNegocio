using static CentralNegocio.Model.Entity.EnumTypes;

namespace CentralNegocio.Application.DTOs
{
    public class adm_permissionDto
    {
        public int? adm_permission_id { get; set; }
        public int? adm_module_id { get; set; }
        public string? path { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? model { get; set; }
        public string? action { get; set; }
        public TypeService? type_service { get; set; }        
    }
}
