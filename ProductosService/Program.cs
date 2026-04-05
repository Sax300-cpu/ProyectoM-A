using Microsoft.EntityFrameworkCore;
using ProductosService.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS para permitir peticiones desde React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Puerto de React
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Conexión a SQL Server
builder.Services.AddDbContext<ProductosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VentasBD")));

// Controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Productos API", Version = "v1" });
});

var app = builder.Build();

// Habilitar Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Habilitar CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();
app.MapControllers();

// Crear la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductosContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
