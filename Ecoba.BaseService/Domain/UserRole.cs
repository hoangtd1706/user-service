namespace Ecoba.BaseService.Domain;

public class UserRole
{
    public string UserNumber { get; private set; }
    public string Role { get; private set; }
    public UserRole(string userNumber, string role)
    {
        this.UserNumber = userNumber;
        this.Role = role.ToUpper();
    }
}
