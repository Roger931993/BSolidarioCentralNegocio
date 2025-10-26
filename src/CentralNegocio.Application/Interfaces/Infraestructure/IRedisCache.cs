namespace CentralNegocio.Application.Interfaces.Infraestructure
{
    public interface IRedisCache
    {
        Task SetAsync<T>(string key, T value, int TimeMinutes = 60);
        Task<T> GetAsync<T>(string key);
    }
}
