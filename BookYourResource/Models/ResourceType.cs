public class ResourceType
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
