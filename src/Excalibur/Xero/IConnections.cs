using Refit;

namespace Excalibur.Xero;

[Headers("Authorization: Bearer")]
public interface IConnections
{
    [Get("/connections")]
    Task<IEnumerable<Tenant>> GetTenants();
}
