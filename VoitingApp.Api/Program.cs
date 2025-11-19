using System.Reflection;
using VoitingApp.Domain;
using VoitingApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();   
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           

builder.Services.AddSingleton<IPolesRepository, PolesRepository>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<PoleEntity, PoleDto>();
    cfg.CreateMap<CreatePoleDto, PoleEntity>();
    cfg.CreateMap<CreateOptionDto, OptionEntity>();
    cfg.CreateMap<OptionEntity, OptionDto>();
    cfg.CreateMap<OptionEntity, OptionResultsDto>();
    cfg.CreateMap<PoleEntity, PoleResultsDto>();
}, Array.Empty<Assembly>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI();
}

app.MapControllers();                

app.Run();

