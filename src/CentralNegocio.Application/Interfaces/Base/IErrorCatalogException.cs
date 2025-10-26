namespace CentralNegocio.Application.Interfaces.Base
{
    public interface IErrorCatalogException
    {
        int CodeError { get; }
        string? MessageError { get; }
        int StatusCodeError { get; }
        Guid? IdTraking { get; set; }
        public string Exception { get; set; }
        Task SetCodeError(int code, Guid? idTraking);
        Task SetCodeError(int code, Guid? idTraking, Exception ex);
        void SetCodeErrorMessage(int code, string? message, Guid? idTraking, int StatusCode = 500);
        void SetCodeErrorMessage(int code, Exception ex, string? message, Guid? idTraking, int StatusCode = 500);
    }
}
