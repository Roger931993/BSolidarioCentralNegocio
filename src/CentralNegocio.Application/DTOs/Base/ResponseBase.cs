using CentralNegocio.Application.Common;

namespace CentralNegocio.Application.DTOs.Base
{
    public class ResponseBase<T>: ResponseBaseError
    {
        public T? data { get; set; }

        public ErrorCatalogException errorCatalogException { get; set; } = null;
        public InfoPages? pages { get; set; }
    }

    public class InfoPages
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages
        {
            get
            {
                if (TotalRecords > 0 && PageSize > 0)
                    return (int)Math.Ceiling((double)TotalRecords / PageSize);
                else
                    return 0;
            }
        }
    }

}
