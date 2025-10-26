using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Catalog;

namespace CentralNegocio.Application.Interfaces.Infraestructure
{
    public interface ICatalogService
    {
        Task<ResponseBase<GetErrorByIdResponse>> GetCatalogErrorById(int id, Guid IdTraker);
    }
}
