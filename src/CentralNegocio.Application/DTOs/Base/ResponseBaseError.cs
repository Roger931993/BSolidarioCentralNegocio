namespace CentralNegocio.Application.DTOs.Base
{
    public class ResponseBaseError
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public bool success { get; set; }
        public int codeError { get; set; }
        public string? messageError { get; set; }
    }
}
