using DustInTheWind.ConsoleTools.Controls.InputControls;
using Excalibur.Authentication;
using Excalibur.Configuration;
using Excalibur.Input;
using Excalibur.Menus;
using Excalibur.Xero;
using Refit;

namespace Excalibur.Workflows;

public abstract class Workflow<T, TRestful>
{
    private readonly IInputReader<T> reader;

    protected Workflow(IInputReader<T> reader)
    {
        this.reader = reader;
    }

    protected abstract Task Upload(TRestful client, string tenantId, T[] items);

    public async Task Process()
    {
        var items = GetItems().ToArray();

        Console.WriteLine("Getting Xero tenants...");

        var token = await Authenticate();
        var tenant = await GetTenant(token);

        while (string.IsNullOrEmpty(tenant))
        {
            token = await Authenticate(true);
            tenant = await GetTenant(token);
        }

        var client = GetClient(token);

        await Upload(client, tenant, items);
    }

    private IEnumerable<T> GetItems()
    {
        var sheets = Directory.GetFiles(Environment.CurrentDirectory, "*.xls*");

        var menu = new SelectMenu<string>("Select input Excel file", sheets, x => x, Path.GetFileName);
        var fileName = menu.GetSelected();

        Console.WriteLine($"Processing {Path.GetFileName(fileName)}");
        Console.WriteLine();

        return reader.ReadItems(fileName);
    }

    private async Task<string> GetTenant(string token)
    {
        var client = GetConnections(token);
        var tenants = await client.GetTenants().ToListAsync();

        tenants.Add(new Tenant {TenantName = "(Add new tenant)"});

        var menu = new SelectMenu<Tenant>("Select Xero tenant", tenants.ToArray(), x => x.TenantId, x => x.TenantName);

        return menu.GetSelected();
    }

    private async Task<string> Authenticate(bool refresh = false)
    {
        var configReader = new ConfigurationReader();
        var config = configReader.Read();

        if (refresh)
        {
            config.RefreshToken = string.Empty;
        }

        if (string.IsNullOrEmpty(config.ClientId))
        {
            var clientIdReader = new StringValue("Enter Xero client id:");
            config.ClientId = clientIdReader.Read();
        }

        var pkceClient = new PkceClient(config);
        var token = await pkceClient.GetToken();

        config.RefreshToken = token!.RefreshToken;

        configReader.Save(config);

        return token.AccessToken;
    }

    private TRestful GetClient(string token)
    {
        var settings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, _) => Task.FromResult(token)
        };

        return RestService.For<TRestful>("https://api.xero.com/api.xro/2.0", settings);
    }

    private IConnections GetConnections(string token)
    {
        var settings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, _) => Task.FromResult(token)
        };

        return RestService.For<IConnections>("https://api.xero.com", settings);
    }
}
