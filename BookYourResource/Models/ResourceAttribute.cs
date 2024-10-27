public class ResourceAttribute
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Value { get; set; }

    public string ResourceId { get; set; }
    public Resource Resource { get; set; } 

}
