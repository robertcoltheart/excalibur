using System.Text.Json.Serialization;

namespace Excalibur.Xero;

public class Contacts
{
    [JsonPropertyName("Contacts")]
    public Contact[] AllContacts { get; set; }
}
