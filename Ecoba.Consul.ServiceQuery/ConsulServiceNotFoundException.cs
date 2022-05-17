using System.Runtime.Serialization;

namespace Ecoba.Consul.ServiceQuery;

[Serializable]
internal class ConsulServiceNotFoundException : Exception
{
    private string v;
    private string serviceName;

    public ConsulServiceNotFoundException()
    {
    }

    public ConsulServiceNotFoundException(string? message) : base(message)
    {
    }

    public ConsulServiceNotFoundException(string v, string serviceName)
    {
        this.v = v;
        this.serviceName = serviceName;
    }

    public ConsulServiceNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ConsulServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}