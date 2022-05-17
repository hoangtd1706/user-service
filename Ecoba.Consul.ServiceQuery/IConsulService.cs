namespace Ecoba.Consul.ServiceQuery;

public interface IConsulService
{
    Task<T> GetAsync<T>(string serviceName, string requestUri, bool withToken = false);
}