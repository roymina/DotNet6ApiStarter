using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiStarter.Entities
{
  public class DataContext : DbContext
  {
    public DbSet<User> Users { get; set; }

    private readonly IConfiguration? configuration;

    public DataContext(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      // in memory database used for simplicity, change to a real db for production applications
      options.UseInMemoryDatabase("TestDb");
    }
  }
}