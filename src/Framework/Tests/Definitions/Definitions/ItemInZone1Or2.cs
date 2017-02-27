namespace N2.Tests.Definitions.Definitions
{
    [PartDefinition("ItemInZone1Or2", AuthorizedRoles = new[] { "Test" })]
    [N2.Integrity.AllowedZones("Zone1", "Zone2")]
    public class ItemInZone1Or2 : N2.ContentItem
    {
    }
}
