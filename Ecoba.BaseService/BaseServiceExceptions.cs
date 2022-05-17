using System.Runtime.Serialization;

namespace Ecoba.BaseService;

[Serializable]
internal class BaseServiceExceptions : Exception
{
    public BaseServiceExceptions()
    {
    }

    public BaseServiceExceptions(string? message) : base(message)
    {
    }
}