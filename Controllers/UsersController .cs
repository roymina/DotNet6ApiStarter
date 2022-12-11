using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiStarter.Authorization;
using ApiStarter.Entities;
using ApiStarter.Models;
using ApiStarter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiStarter.Controllers
{
  [Authorized]
  [ApiController]
  [Route("[controller]")]
  public class UsersController : ControllerBase
  {

    private readonly IUserService userService;
    public UsersController(IUserService userService)
    {
      this.userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    public IActionResult Authenticate(AuthReqModel model)
    {
      var response = userService.Authenticate(model);
      return Ok(response);
    }
    [Authorized(Role.Admin)]
    [HttpGet]
    public IActionResult GetAll()
    {
      var users = userService.GetAll();
      return Ok(users);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
      // only admins can access other user records
      var currentUser = (User)HttpContext.Items["User"]!;
      if (id != currentUser.Id && currentUser.Role != Role.Admin)
        return Unauthorized(new { message = "Unauthorized" });

      var user = userService.GetById(id);
      return Ok(user);
    }

  }
}