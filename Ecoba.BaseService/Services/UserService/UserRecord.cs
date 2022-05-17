namespace Ecoba.BaseService.Services.UserService;

public record UserRecord
{
    public string Username { get; set; }
    public string EmployeeId { get; set; }
    public string DisplayName { get; set; }
}