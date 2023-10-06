using Newtonsoft.Json;

namespace VebTestTask.Models;

public class Role
{
    public int Id { get; set; }
    public string? Name { get; set; }
    [JsonIgnore]
    public ICollection<User> Users { get; set; }
}