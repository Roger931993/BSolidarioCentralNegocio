using CentralNegocio.Application.Interfaces.Persistence;
using CentralNegocio.Domain.Interfaces.Dapper;
using CentralNegocio.Persistence.Contexts;
using CentralNegocio.Persistence.Repositories.Dapper.Common;

namespace CentralNegocio.Persistence.Repositories.Dapper
{
    public class UnitOfWorkStamp : UnitOfWork, IUnitOfWorkStamp
    {
        private readonly IDatabaseConnect _options;
        public UnitOfWorkStamp(ClientesContextCommand contextDapper, IDatabaseConnect options) : base(contextDapper)
        {
            _options = options;
        }
    }
}
