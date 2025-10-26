using CentralNegocio.Domain.Common;
using System.Text.Json;

namespace CentralNegocio.Infrastructure.Services.Common
{
    public class BaseService
    {
        public async Task AddLogInput<TRequest>(TRequest? request, int statusCode, DataCacheLocal dataCache)
        {
            // Crear un objeto XmlSerializer para la clase TRequest
            string jsonRequest = JsonSerializer.Serialize(request);

            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "In",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = jsonRequest,
            });
        }

        public async Task AddLogInput(string request, int statusCode, DataCacheLocal dataCache)
        {          
            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "In",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = request,
            });
        }

        public async Task AddLogError<TRequest>(TRequest? request, int statusCode, Exception ex, DataCacheLocal dataCache)
        {
            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "Error",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = $"{Newtonsoft.Json.JsonConvert.SerializeObject(ex, Newtonsoft.Json.Formatting.Indented)}",
            });
        }      

        public async Task AddLogOutput<TResponse>(TResponse? response, int statusCode, DataCacheLocal dataCache)
        {
            string jsonResponse = JsonSerializer.Serialize(response);

            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "Out",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = jsonResponse,
            });
        }

        public async Task AddLogOutput<TResponse>(string? response, int statusCode, DataCacheLocal dataCache)
        {
            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "Out",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = response,
            });
        }
    }
}