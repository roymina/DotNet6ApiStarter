using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiStarter.Entities;

namespace ApiStarter.Models
{
  public class AuthReqModel
  {
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
  }
  public class AuthResModel
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public Role Role { get; set; }
    public string Token { get; set; }

    public AuthResModel(User user, string token)
    {
      Id = user.Id;
      Username = user.Username;
      Role = user.Role;
      Token = token;
    }
  }
}