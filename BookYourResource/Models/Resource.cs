public class Resource
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string CodeName { get; set; }
    public string Details { get; set; }
    public string TypeId { get; set; }
    public ResourceType ResourceType  { get; set; }
  
    public ResourceAttribute ResourceAttributeEntity { get; set; }

    public ICollection<ResourceAttribute> ResourceAttributes { get; set; } = new List<ResourceAttribute>();

     public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
     

}
