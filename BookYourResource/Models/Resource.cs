public class Resource
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string CodeName { get; set; }
    public string Details { get; set; }
    public string Type { get; set; }
    public ResourceType ResourceTypeEntity { get; set; }
    public string Attribute { get; set; }
    public ResourceAttribute ResourceAttributeEntity { get; set; }
}
