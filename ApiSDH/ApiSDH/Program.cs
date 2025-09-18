using ApiSDH.Common.Services;
using ApiSDH.DI;
using ApiSDH.MIddleware;
using Application.Common.Interfaces.Persistence;
using Application.DI;
using Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Host);
builder.Services.AddPresentation(builder.Configuration);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();
    ConfigEnforcementService.EnsureSingleConfig(db);
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();