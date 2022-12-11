/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
*/


using System.Text.Json.Serialization;
using ApiStarter.Authorization;
using ApiStarter.Entities;
using ApiStarter.Services;
using ApiStarter.Utils;
using BCryptNet = BCrypt.Net.BCrypt;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
  var services = builder.Services;
  var env = builder.Environment;

  services.AddDbContext<DataContext>();
  services.AddCors();
  services.AddControllers().AddJsonOptions(x =>
  {
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });

  // configure strongly typed settings object
  services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

  // configure DI for application services
  services.AddScoped<IJwtService, JwtService>();
  services.AddScoped<IUserService, UserService>();
}

var app = builder.Build();

// configure HTTP request pipeline
{
  // global cors policy
  app.UseCors(x => x
      .AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader());

  // global error handler
  app.UseMiddleware<ErrorHandlerMiddleware>();

  // custom jwt auth middleware
  app.UseMiddleware<JwtMiddleware>();

  app.MapControllers();
}

// create hardcoded test users in db on startup
{
  var testUsers = new List<User>
    {
        new User { Id = 1 , Username = "admin", PasswordHash = BCryptNet.HashPassword("admin"), Role = Role.Admin },
        new User { Id = 2 , Username = "user", PasswordHash = BCryptNet.HashPassword("user"), Role = Role.User }
    };

  using var scope = app.Services.CreateScope();
  var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
  dataContext.Users.AddRange(testUsers);
  dataContext.SaveChanges();
}

app.Run("http://localhost:4000");
