using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Caching.Memory;

using Ecoba.UserService.Model;
using Newtonsoft.Json.Linq;

namespace Ecoba.UserService.Controllers;

[ApiController]
[Route("api/v1/users")]
// [Authorize]
public class UserController : ControllerBase
{
    private readonly string URI_MS_GRAPH_API = "https://graph.microsoft.com/v1.0";
    private IMemoryCache _memoryCache;
    public UserController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    [HttpGet("test")]
    public string Index()
    {
        return "test";
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var end_point = $"{URI_MS_GRAPH_API}/users?$select=displayName,givenName,jobTitle,mail,mobilePhone,officeLocation,preferredLanguage,surname,userPrincipalName,id,employeeID";
        end_point += "&$filter=startsWith(employeeID,'-1') or startsWith(employeeID,'0') or startsWith(employeeID,'1') or startsWith(employeeID,'2') or startsWith(employeeID,'3') or startsWith(employeeID,'4') or startsWith(employeeID,'5') or startsWith(employeeID,'6') or startsWith(employeeID,'7') or startsWith(employeeID,'8') or startsWith(employeeID,'9')";
        if (Request.Headers.TryGetValue("Authorization", out var tokenAzure))
        {
            IEnumerable<User> users;
            if (!_memoryCache.TryGetValue("users", out users))
            {
                var token = tokenAzure.ToString().Substring(tokenAzure.ToString().IndexOf(" ") + 1);
                users = await GetUser(end_point, token);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set("users", users, cacheEntryOptions);
            }
            if (users != null) return Ok(users);
        }
        return BadRequest(end_point);
    }

    private async Task<IEnumerable<User>> GetUser(string uri, string token)
    {
        HttpClient _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await _client.GetAsync(uri);
        var listResult = new List<User>();
        if (response.IsSuccessStatusCode)
        {
            var raw = JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result);
            var res = await response.Content.ReadAsAsync<ResponseAzure>();
            listResult.AddRange(res.Value);
            if (raw["@odata.nextLink"] != null)
            {
                var nextList = await GetUser(raw["@odata.nextLink"].ToString(), token);
                if (nextList != null) listResult.AddRange(nextList);
            }
            return listResult;
        }
        return null;
    }

    [HttpGet("me")]
    public async Task<ActionResult> GetProfile()
    {
        var end_point = $"{URI_MS_GRAPH_API}/me?$select=displayName,givenName,jobTitle,mail,mobilePhone,officeLocation,preferredLanguage,surname,userPrincipalName,id,employeeID";
        HttpClient _client = new HttpClient();
        if (Request.Headers.TryGetValue("Authorization", out var tokenAzure))
        {
            var token = tokenAzure.ToString().Substring(tokenAzure.ToString().IndexOf(" ") + 1);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync(end_point);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsAsync<User>();
                return Ok(res);
            }
        }
        return BadRequest();
    }

    [HttpGet("photo")]
    public async Task<ActionResult> GetPhoto()
    {
        if (Request.Headers.TryGetValue("Authorization", out var tokenAzure))
        {
            var token = tokenAzure.ToString().Substring(tokenAzure.ToString().IndexOf(" ") + 1);
            var end_point = $"{URI_MS_GRAPH_API}/me/photo/$value";
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync(end_point);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStreamAsync();
                return Ok(res);
            }
        }
        return BadRequest();
    }
}