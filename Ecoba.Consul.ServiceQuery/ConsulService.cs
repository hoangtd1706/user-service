using Consul;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Ecoba.Consul.ServiceQuery;

public class ConsulService : IConsulService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _client;
    private IConsulClient _consulClient;

    public ConsulService(HttpClient client, IConsulClient consulClient, IConfiguration config)
    {
        _client = client;
        _consulClient = consulClient;
        _config = config;
    }

    public async Task<T> GetAsync<T>(string serviceName, string requestUri, bool withToken = false)
    {
        // var uri = await GetRequestUriAsync(serviceName, requestUri);
        // if (withToken)
        // {
        //     _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + new AccessToken(_config).GenerateToken().ToString());
        // }
        // var response = await _client.GetAsync(uri);

        // if (!response.IsSuccessStatusCode)
        // {
        //     return default(T);
        // }

        // var content = await response.Content.ReadAsStringAsync();
        // return JsonConvert.DeserializeObject<T>(content);
        var uri = await GetRequestUriAsync(serviceName, requestUri);

        if (withToken)
        {
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + new AccessToken(_config).GenerateToken().ToString());
        }

        var response = await _client.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            return default(T);
        }

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content);
    }

    private async Task<string> GetRequestUriAsync(string serviceName, string uri)
    {
        //Get all services registered on Consul
        var allRegisteredServices = await _consulClient.Agent.Services();
        var registeredServices = allRegisteredServices.Response?.Where(s => s.Value.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).ToList();
        if (registeredServices != null)
        {
            var service = GetRandomInstance(registeredServices, serviceName);
            if (service == null)
            {
                throw new Exception($"Consul service: '{serviceName}' was not found.");
            }
            return $"http://{service.Address}:{service.Port}/{uri}";
        }
        return "";
    }

    private AgentService GetRandomInstance(IList<AgentService> services, string serviceName)
    {
        Random _random = new Random();

        AgentService serviceToUse = null;

        serviceToUse = services[_random.Next(0, services.Count)];

        return serviceToUse;
    }
}