namespace Excalibur.Xero;

public class PurchaseOrder
{
    public Contact Contact { get; set; }

    public string? Date { get; set; }

    public string? DeliveryDate { get; set; }

    public string? Reference { get; set; }

    public string? Status { get; set; }

    public List<PurchaseOrderLineItem> LineItems { get; set; } = new();
}
