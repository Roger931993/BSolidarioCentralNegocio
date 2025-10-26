using CentralNegocio.Application.DTOs.Base;

namespace CentralNegocio.Application.Interfaces.Base
{
    public interface IRequestBase
    {
        Guid? IdTraking { get; set; }
        InfoSessionDto? InfoSession { get; set; }
    }
}
