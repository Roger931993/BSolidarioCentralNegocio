namespace CentralNegocio.Domain.Common
{
    public class DataCacheLocalProcess
    {
        public DateTime CreatedAt { get; set; }
        public string TypeProcess { get; set; } = string.Empty;
        public string DataMessage { get; set; } = string.Empty;
        public string ProcessComponent { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
