using Askly.Api.Extensions;
using Askly.Api.Handlers;
using Askly.Api.Middleware;
using Askly.Application.Interfaces.Auth;
using Askly.Application.Interfaces.Repositories;
using Askly.Application.Interfaces.Services;
using Askly.Application.Profiles;
using Askly.Infrastructure.Repositories;
using Askly.Application.Services;
using Askly.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddApiAuthentication(configuration);

builder.Services.AddControllers();   

builder.Services.AddEndpointsApiExplorer(); 

builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IPollsService, PollsService>();

builder.Services.AddScoped<IPollsRepository, PollsRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddScoped<IVotesRepository, VotesRepository>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => Results.Redirect("/polls"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI();
}
app.UseRouting();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Outgoing: {context.Response.StatusCode}");
});

// app.UseMiddleware<AnonymousUserMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();                

app.UseStaticFiles();

app.MapRazorPages();

app.Run();

