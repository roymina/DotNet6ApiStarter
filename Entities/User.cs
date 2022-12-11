using System.Text.Json.Serialization;

namespace ApiStarter.Entities
{
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public Role Role { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
  }
}