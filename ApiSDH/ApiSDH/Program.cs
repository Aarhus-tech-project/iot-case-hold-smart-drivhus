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
builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration, builder.Host);

builder.Services.AddSignalR(); // required? 

if (builder.Environment.IsDevelopment()) builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();
    UserEnforcementService.EnsureSingleUser(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.Run();