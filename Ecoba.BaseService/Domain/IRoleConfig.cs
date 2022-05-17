namespace Ecoba.BaseService.Domain;

public interface IRoleConfig
{
    Task<IEnumerable<string>> GetAllAsync();
}