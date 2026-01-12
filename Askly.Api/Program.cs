using Askly.Application.Interfaces.Repositories;
using Askly.Application.Profiles;
using Askly.Domain.Entities;
using Askly.Infrastructure.Repositories;
using Askly.Application.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
// });

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});


builder.Services.AddControllers();   

builder.Services.AddEndpointsApiExplorer(); 

builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IPollService, PollService>();

builder.Services.AddSingleton<IPollsRepository, InMemoryPollsRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

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


app.MapControllers();                

app.UseStaticFiles();

app.MapRazorPages();

app.Run();

