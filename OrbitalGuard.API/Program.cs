using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OrbitalGuard.API.Data;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString,
        b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    const int maxTentativas = 30;
    for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
    {
        try
        {
            logger.LogInformation("Aplicando Migrations no Oracle (tentativa {Tentativa}/{Max})...",
                                  tentativa, maxTentativas);
            db.Database.Migrate();
            logger.LogInformation("Migrations aplicadas com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Banco ainda nao disponivel: {Msg}. Nova tentativa em 10s.", ex.Message);
            if (tentativa == maxTentativas)
            {
                logger.LogError(ex, "Nao foi possivel aplicar as Migrations apos {Max} tentativas.", maxTentativas);
                throw;
            }
            Thread.Sleep(10_000);
        }
    }
}


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrbitalGuard API v1");
    
    c.RoutePrefix = string.Empty;
});


if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();