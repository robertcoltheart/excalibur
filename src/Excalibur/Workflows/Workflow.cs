using DustInTheWind.ConsoleTools.Controls.InputControls;
using Excalibur.Authentication;
using Excalibur.Configuration;

namespace Excalibur.Workflows;

public abstract class Workflow
{
    protected async Task<ExcaliburConfiguration> Authenticate()
    {
        var configReader = new ConfigurationReader();
        var config = configReader.Read();

        if (string.IsNullOrEmpty(config.ClientId))
        {
            var clientIdReader = new StringValue("Enter Xero client id:");
            config.ClientId = clientIdReader.Read();
        }

        var pkceClient = new PkceClient(config);
        var token = await pkceClient.GetToken();

        config.RefreshToken = token!.RefreshToken;

        configReader.Save(config);

        return config;
    }
}
