namespace Excalibur.Input;

public class LineItem
{
    public string? PurchaseOrderNumber { get; set; }

    public string? Contact { get; set; }

    public DateTime? Date { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? Reference { get; set; }

    public string? Status { get; set; }

    public string? AccountCode { get; set; }

    public string? Description { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitAmount { get; set; }

    public string? TaxType { get; set; }
}
