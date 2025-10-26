using CentralNegocio.Domain.Interfaces.Dapper;
using CentralNegocio.Persistence.Repositories.Dapper.Common;

namespace CentralNegocio.Persistence.Contexts
{
    public class ClientesContextCommand : DbContextDapperCommon
    {
        public ClientesContextCommand(IDatabaseConnect options) : base(options.GetConnection("Clientes"))
        {
        }
    }
}
