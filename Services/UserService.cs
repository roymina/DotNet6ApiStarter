using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiStarter.Entities;
using ApiStarter.Models;
using ApiStarter.Utils;
using Microsoft.Extensions.Options;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ApiStarter.Services
{
  public interface IUserService
  {
    AuthResModel Authenticate(AuthReqModel model);
    IEnumerable<User> GetAll();
    User GetById(int id);
  }
  public class UserService : IUserService
  {
    private DataContext context;
    private IJwtService jwtService;
    private readonly AppSettings appSettings;

    public UserService(
        DataContext context,
        IJwtService jwtService,
        IOptions<AppSettings> appSettings)
    {
      this.context = context;
      this.jwtService = jwtService;
      this.appSettings = appSettings.Value;
    }

    //授权
    public AuthResModel Authenticate(AuthReqModel model)
    {
      var user = context.Users.SingleOrDefault(x => x.Username == model.Username);

      // validate
      if (user == null || !BCryptNet.Verify(model.Password, user.PasswordHash))
        throw new AppException("Username or password is incorrect");

      // authentication successful so generate jwt token
      var jwtToken = jwtService.GenerateJwtToken(user);

      return new AuthResModel(user, jwtToken);
    }

    public IEnumerable<User> GetAll()
    {
      return context.Users;
    }

    public User GetById(int id)
    {
      var user = context.Users.Find(id);
      if (user == null) throw new KeyNotFoundException("User not found");
      return user;
    }
  }
}