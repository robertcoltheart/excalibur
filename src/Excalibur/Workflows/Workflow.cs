using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using DustInTheWind.ConsoleTools.Controls.InputControls;
using Excalibur.Authentication;
using Excalibur.Configuration;
using Excalibur.Input;
using Excalibur.Menus;
using Excalibur.Xero;
using Pastel;
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

        if (!items.Any())
        {
            Console.WriteLine("No valid Excel files found in current folder");

            return;
        }

        Console.WriteLine("Getting Xero tenants...".Pastel(ConsoleColor.Gray));

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

        if (!sheets.Any())
        {
            return Enumerable.Empty<T>();
        }

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
            var clientIdReader = new StringValue("Enter Xero client id:".Pastel(ConsoleColor.Yellow));
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
        var serializer = SystemTextJsonContentSerializer.GetDefaultJsonSerializerOptions();
        serializer.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        var settings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, _) => Task.FromResult(token),
            ContentSerializer = new SystemTextJsonContentSerializer(serializer),
            ExceptionFactory = HandleValidationException
        };

        return RestService.For<TRestful>("https://api.xero.com/api.xro/2.0", settings);
    }

    private IConnections GetConnections(string token)
    {
        var settings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, _) => Task.FromResult(token),
        };

        return RestService.For<IConnections>("https://api.xero.com", settings);
    }

    private async Task<Exception?> HandleValidationException(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        var error = await response.Content.ReadFromJsonAsync<Error>();

        var messages = error!.Elements
            .SelectMany(x => x.ValidationErrors)
            .Select(x => x.Message)
            .Distinct();

        var builder = new StringBuilder()
            .AppendLine("The following validation errors occurred:");

        foreach (var message in messages)
        {
            builder.AppendLine(message);
        }

        return new InvalidOperationException(builder.ToString());
    }
}
