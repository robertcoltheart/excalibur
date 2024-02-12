﻿namespace Excalibur.Xero;

public class PurchaseOrderLineItem
{
    public string Description { get; set; }

    public string Quantity { get; set; }

    public string UnitAmount { get; set; }

    public string ItemCode { get; set; }

    public string AccountCode { get; set; }

    public string TaxType { get; set; }

    public string DiscountRate { get; set; }

    public string Tracking { get; set; }
}
