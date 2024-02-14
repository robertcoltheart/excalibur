using Excalibur.Input;
using Excalibur.Xero;
using Pastel;

namespace Excalibur.Workflows;

public class PurchaseOrderWorkflow : Workflow<LineItem, IAccounting>
{
    public PurchaseOrderWorkflow(IInputReader<LineItem> reader)
        : base(reader)
    {
    }

    protected override async Task Upload(IAccounting client, string tenant, LineItem[] items)
    {
        var contacts = await GetContacts(client, tenant).ToArrayAsync();

        var purchaseGroups = items
            .GroupBy(x => new {x.PurchaseOrderNumber, x.Contact, x.Date, x.DeliveryDate, x.Reference, x.Status});

        foreach (var purchaseGroup in purchaseGroups)
        {
            var contact = contacts.FirstOrDefault(x => x.Name == purchaseGroup.Key.Contact);

            if (contact == null)
            {
                Console.WriteLine($"ERROR: Contact not found: {purchaseGroup.Key.Contact}".Pastel(ConsoleColor.Red));

                return;
            }

            var purchaseOrder = new PurchaseOrder
            {
                Contact = new Contact
                {
                    ContactId = contact!.ContactId
                },
                Date = purchaseGroup.Key.Date?.ToString("yyyy-MM-dd"),
                DeliveryDate = purchaseGroup.Key.DeliveryDate?.ToString("yyyy-MM-dd"),
                Reference = purchaseGroup.Key.Reference,
                Status = purchaseGroup.Key.Status?.ToUpper(),
                LineItems = purchaseGroup.Select(CreateLineItem).ToList()
            };

            Console.WriteLine($"Creating purchase order: Contact:{contact.Name}, Date:{purchaseOrder.Date}, DeliveryDate:{purchaseOrder.DeliveryDate}, Ref:{purchaseOrder.Reference}, Status:{purchaseOrder.Status}".Pastel(ConsoleColor.Gray));

            try
            {
                await client.CreatePurchaseOrder(tenant, purchaseOrder);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}".Pastel(ConsoleColor.Red));

                return;
            }
        }

        Console.WriteLine("Completed creating purchase orders".Pastel(ConsoleColor.Green));
    }

    private PurchaseOrderLineItem CreateLineItem(LineItem lineItem)
    {
        var item = new PurchaseOrderLineItem
        {
            AccountCode = lineItem.AccountCode,
            Description = lineItem.Description,
            Quantity = lineItem.Quantity,
            UnitAmount = lineItem.UnitAmount,
            TaxType = lineItem.TaxType!.ToUpper()
        };

        if (!string.IsNullOrEmpty(lineItem.TrackingCategoryOption))
        {
            item.Tracking = new PurchaseOrderLineItemTrackingCategory[]
            {
                new()
                {
                    Name = lineItem.TrackingCategoryName,
                    Option = lineItem.TrackingCategoryOption
                }
            };
        }

        return item;
    }

    private async Task<IEnumerable<Contact>> GetContacts(IAccounting client, string tenant)
    {
        var contacts = await client.GetContacts(tenant);

        return contacts
            .AllContacts
            .Where(x => x.ContactStatus == ContactStatus.Active)
            .OrderBy(x => x.Name);
    }
}
