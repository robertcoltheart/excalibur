using Refit;

namespace Excalibur.Xero;

[Headers("Authorization: Bearer")]
public interface IAccounting
{
    [Get("/PurchaseOrders")]
    Task<PurchaseOrder> GetPurchaseOrders([Header("xero-tenant-id")] string tenantId);

    [Post("/PurchaseOrders")]
    Task<string> CreatePurchaseOrder([Header("xero-tenant-id")] string tenantId, PurchaseOrder purchaseOrder);

    [Get("/Contacts")]
    Task<Contacts> GetContacts([Header("xero-tenant-id")] string tenantId);

    [Get("/TrackingCategories")]
    Task<Tracking> GetTrackingCategories([Header("xero-tenant-id")] string tenantId);
}
