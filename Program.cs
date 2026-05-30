using ApiTareas.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto asegura que todos los enums se serialicen como strings en el JSON de entrada y salida
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Registrar el DbContext con SQLite usando la cadena de conexión de appsettings.json
builder.Services.AddDbContext<TareasDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar HttpClient tipado para el servicio de tareas externas
builder.Services.AddHttpClient<ApiTareas.Services.ITareasExternasService, ApiTareas.Services.TareasExternasService>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
});

// Registrar el servicio de ML.NET como Singleton (el modelo se entrena una sola vez al iniciar)
builder.Services.AddSingleton<ApiTareas.Services.IMlService, ApiTareas.Services.MlService>();


var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar (necesario para Render/Docker)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TareasDbContext>();
    db.Database.Migrate();
}

// Habilitar Swagger en todos los entornos (para que funcione en Render)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();

