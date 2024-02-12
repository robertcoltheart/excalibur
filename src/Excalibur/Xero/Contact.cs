namespace Excalibur.Xero;

public class Contact
{
    public string ContactId { get; set; }

    public ContactStatus ContactStatus { get; set; }

    public string Name { get; set; }

    public bool IsSupplier { get; set; }

    public bool IsCustomer { get; set; }
}
