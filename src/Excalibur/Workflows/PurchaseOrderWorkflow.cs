using DustInTheWind.ConsoleTools.Controls.Menus;
using Excalibur.Input;
using Excalibur.Menus;
using Excalibur.Xero;

namespace Excalibur.Workflows;

public class PurchaseOrderWorkflow : Workflow<LineItem, IAccounting>
{
    public PurchaseOrderWorkflow(IInputReader<LineItem> reader)
        : base(reader)
    {
    }

    protected override async Task Upload(IAccounting client, string tenant, LineItem[] items)
    {
        var contact = await GetContact(client, tenant);

        var order = new PurchaseOrder();

        foreach (var item in items)
        {
            var lineItem = new PurchaseOrderLineItem
            {
                Description = item.Description
            };
        }

        await client.CreatePurchaseOrder(tenant, order);
    }

    private async Task<string> GetContact(IAccounting client, string tenant)
    {
        var contacts = await client.GetContacts(tenant);

        var activeContacts = contacts
            .AllContacts
            .Where(x => x.ContactStatus == ContactStatus.Active)
            .OrderBy(x => x.Name)
            .ToArray();

        var menu = new SelectMenu<Contact>("Select purchase order contact", activeContacts, x => x.ContactId, x => x.Name);

        return menu.GetSelected();
    }
}
