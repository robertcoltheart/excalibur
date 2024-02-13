namespace Excalibur.Xero;

public class PurchaseOrderLineItem
{
    public string? Description { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitAmount { get; set; }

    public string? ItemCode { get; set; }

    public string? AccountCode { get; set; }

    public string? TaxType { get; set; }

    public string? DiscountRate { get; set; }

    public string? Tracking { get; set; }
}
