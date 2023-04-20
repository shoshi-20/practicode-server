using Microsoft.EntityFrameworkCore;
using TodoApi;
// using Microsoft.OpenApi.Models;
// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.HttpOverrides;
// using Microsoft.IdentityModel.Tokens;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Identity.Web;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authorization.Policy;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
     builder => builder.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
});
builder.Services.AddSwaggerGen();
//     options =>
// {
    // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    // {
    //     Scheme = "Bearer",
    //     BearerFormat = "JWT",
    //     In = ParameterLocation.Header,
    //     Name = "Authorization",
    //     Description = "Bearer Authentication with JWT Token",
    //     Type = SecuritySchemeType.Http
    // });
    // options.AddSecurityRequirement(new OpenApiSecurityRequirement
    // {
    //     {
    //         new OpenApiSecurityScheme
    //         {
    //     Reference = new OpenApiReference
    //             {
    //                 Id = "Bearer",
    //                 Type = ReferenceType.SecurityScheme
    //             }
    //         },
    //         new List<string>()
    //     }
    // });
// }

// builder.Services.AddAuthentication(
//     options =>
// {
//     // options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }
// )
    //  .AddJwtBearer(
    //   options =>
    // {
    //     options.TokenValidationParameters = new TokenValidationParameters
    //     {
    //         ValidateIssuer = true,
    //         ValidateAudience = true,
    //         ValidateLifetime = true,
    //         ValidateIssuerSigningKey = true,
    //         ValidIssuer = builder.Configuration["JWT:Issuer"],
    //         ValidAudience = builder.Configuration["JWT:Audience"],
    //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    //     };
    // }
    // );
// builder.Services.AddAuthorizationBuilder().AddPolicy("admin_greetings", policy =>
//         policy
//             .RequireRole("admin")
//             .RequireScope("greetings_api"));
var app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
app.UseCors("CorsPolicy");

app.MapGet("/todoitems", async (ToDoDbContext context) =>
{

   return await context.Items.ToListAsync();
});
// app.MapGet("/users", async (ToDoDbContext context) =>
// {

//    return await context.Users.ToListAsync();
// });
app.MapPost("/todoitems", async (Item todo, ToDoDbContext context) =>
{
    context.Items.Add(todo);
    await context.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});
// app.MapPost("/users", async (User user, ToDoDbContext context) =>
// {
//     context.Users.Add(user);
//     await context.SaveChangesAsync();
//     return Results.Created($"/todoitems/{user.UserId}", user);
// });

app.MapPut("/todoitems/{id}", async (int id, Boolean isComplete, ToDoDbContext context) =>
{
    var newItem = await context.Items.FindAsync(id);
    if (newItem is null) return Results.NotFound();
    newItem.IsComplete = isComplete;
    await context.SaveChangesAsync();
    return Results.Ok(newItem);
});

app.MapDelete("/todoitems/{id}", async (int id, ToDoDbContext context) =>
{
    if (await context.Items.FindAsync(id) is Item todo)
    {
        context.Items.Remove(todo);
        await context.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
});
app.MapGet("/",()=>"practicode server is running...");
app.Run();
// public class AuthController : ControllerBase
// {
//     private readonly IConfiguration _configuration;
//     private readonly ToDoDbContext _dataContext;

//     public AuthController(IConfiguration configuration, ToDoDbContext dataContext)
//     {
//         _configuration = configuration;
//         _dataContext = dataContext;
//     }
//     [HttpPost("/login")]
//     public IActionResult Login([FromBody] User loginModel)
//     {
//         var user = _dataContext.Users?.FirstOrDefault(u => u.Name == loginModel.Name && u.Password == loginModel.Password);
//         if (user is not null)
//         {
//             var jwt = CreateJWT(user);
//             // AddSession(user);
//             return Ok(jwt);
//         }
//         return Unauthorized();
//     }
//     [HttpPost("/api/register")]
//     public IActionResult Register([FromBody] User loginModel)
//     {
//         // var name = loginModel.Email.Split("@")[0];
//         var lastId = _dataContext.Users?.Max(u => u.UserId) ?? 0;
//         var newUser = new User { UserId = lastId + 1, Name = loginModel.Name, Password = loginModel.Password, Role = "temp_user" };
//         _dataContext.Users?.Add(newUser);
//         var jwt = CreateJWT(newUser);
//         // AddSession(newUser);
//         return Ok(jwt);
//     }
//     private object CreateJWT(User user)
//     {
//         var claims = new List<Claim>()
//                 {
//                     new Claim("id", user.UserId.ToString()),
//                     new Claim("name", user.Name),
//                     new Claim("role", user.Role)
//                 };

//         var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
//         var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
//         var tokeOptions = new JwtSecurityToken(
//             issuer: _configuration.GetValue<string>("JWT:Issuer"),
//             audience: _configuration.GetValue<string>("JWT:Audience"),
//             claims: claims,
//             expires: DateTime.Now.AddDays(30),
//             signingCredentials: signinCredentials
//         );
//         var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
//         return new { Token = tokenString };
//     }

//     // private void AddSession(User user)
//     // {
//     //     var lastId = _dataContext.Sessions?.Max(u => u.UserId) ?? 0;
//     //     _dataContext.Sessions?.Add(new Session { Id = lastId + 1, UserId = user.UserId, DateTime = DateTime.Now.ToString(), IP = Request.HttpContext.Connection.RemoteIpAddress.ToString(), IsValid = true });
//     // }

// }
// public class CustomAuthorizationHandler : IAuthorizationMiddlewareResultHandler
//     {
//         private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

//         public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
//         {
//             int.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value, out int userId);

//             context.User.AddIdentity(new CustomIdentity
//             {
//                 Id = userId,
//                 UserName = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
//                 Email = context.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
//                 Role = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value
//             });

//             await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
//         }
//     }
