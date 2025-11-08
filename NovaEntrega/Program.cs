// Imports principais usados pela API (EF Core, Serilog, etc)
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Serilog;
using Servidor_PI.Data;
using Servidor_PI.Repositories;
using Servidor_PI.Repositories.Interfaces;
using Servidor_PI.Enums;
using Servidor_PI.Models;

// Cria o builder da aplicacao (configuracoes e servicos)
var builder = WebApplication.CreateBuilder(args);

// Configura logs no console (Serilog)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Diz para a aplicacao usar o Serilog que acabamos de configurar
builder.Host.UseSerilog();

// Adiciona controllers (endpoints da API)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Deixar enums como texto (ex.: Pendente)
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Evita erro com referencias circulares
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Documentacao da API (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco de dados (SQLite) usando connection string do appsettings
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Registra os repositorios (acesso ao banco)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICampanhaRepository, CampanhaRepository>();
builder.Services.AddScoped<IDoacaoRepository, DoacaoRepository>();
builder.Services.AddScoped<INoticiasRepository, NoticiasRepository>();
builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();

// CORS (permite chamadas de outros dominios)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Constroi a aplicacao com as configuracoes
var app = builder.Build();

// Middleware e Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Inicializa o banco com Migrations (sem usar schema.sql)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();             // aplica migrations pendentes
        await db.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;"); // habilita FKs (SQLite)
    }

    Log.Information("Banco migrado e aplicação iniciada com sucesso!");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro ao configurar/migrar banco de dados");
    throw;
}

app.Run();

