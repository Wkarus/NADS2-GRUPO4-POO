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
        // Garante que a pasta Data exista para o SQLite (evita erro 14 em publish/Azure)
        var dataDir = Path.Combine(app.Environment.ContentRootPath, "Data");
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();                               // aplica migrations pendentes
        await db.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;"); // habilita FKs (SQLite)

        // Seed de dados de exemplo (somente se vazio)
        if (!await db.Usuarios.AnyAsync())
        {
            var u1 = new Usuario
            {
                nome_completo = "João Silva Santos",
                nome_usuario = "joao123",
                email = "joao@example.com",
                senha = "123456",
                telefone = "11999990000",
                cpf = "12345678901",
                cep = "01000-000"
            };

            var u2 = new Usuario
            {
                nome_completo = "Maria Oliveira",
                nome_usuario = "maria456",
                email = "maria@example.com",
                senha = "123456",
                telefone = "11988880000",
                cpf = "98765432100",
                cep = "02000-000"
            };

            await db.Usuarios.AddRangeAsync(u1, u2);
            await db.SaveChangesAsync();
        }

        if (!await db.Campanhas.AnyAsync())
        {
            var c1 = new Campanha
            {
                nome_campanha = "Campanha do Agasalho 2024",
                meta_arrecadacao = 10000m,
                inicio = DateTime.UtcNow.AddDays(-30),
                fim = DateTime.UtcNow.AddDays(60)
            };

            var c2 = new Campanha
            {
                nome_campanha = "Natal Solidário",
                meta_arrecadacao = 20000m,
                inicio = DateTime.UtcNow.AddDays(-10),
                fim = DateTime.UtcNow.AddDays(90)
            };

            await db.Campanhas.AddRangeAsync(c1, c2);
            await db.SaveChangesAsync();
        }

        if (!await db.Doacoes.AnyAsync())
        {
            var usuarioIds = await db.Usuarios.Select(u => u.cd_cliente).ToListAsync();
            var campanhaIds = await db.Campanhas.Select(c => c.cd_campanha).ToListAsync();
            if (usuarioIds.Count >= 2 && campanhaIds.Count >= 2)
            {
                var d1 = new Doacao
                {
                    cd_cliente = usuarioIds[0],
                    cd_campanha = campanhaIds[0],
                    nome_doacao = "Doação de roupas", 
                    tipo_doacao = TipoDoacao.Roupas,
                    forma_arrecadacao = FormaArrecadacao.Entrega,
                    status_arrecadacao = StatusArrecadacao.Pendente
                };

                var d2 = new Doacao
                {
                    cd_cliente = usuarioIds[1],
                    cd_campanha = campanhaIds[1],
                    nome_doacao = "Doação em dinheiro", 
                    tipo_doacao = TipoDoacao.Dinheiro,
                    forma_arrecadacao = FormaArrecadacao.PIX,
                    status_arrecadacao = StatusArrecadacao.Confirmada
                };

                await db.Doacoes.AddRangeAsync(d1, d2);
                await db.SaveChangesAsync();
            }
        }
    }

    Log.Information("Banco migrado e aplicação iniciada com sucesso!");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro ao configurar/migrar banco de dados");
    throw;
}

app.Run();

