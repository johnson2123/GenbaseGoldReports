using GenbaseGoldReports.Api.Extensions;
using GenbaseGoldReports.Api.Middleware;
using GenbaseGoldReports.Application;
using GenbaseGoldReports.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilog();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityAndAuthSetup(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    await app.SeedMasterDatabaseAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCorsPolicy();

app.UseMiddleware<TenantResolverMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting GenbaseGoldReports Web API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "GenbaseGoldReports Web API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}