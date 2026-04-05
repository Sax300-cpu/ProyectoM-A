using ClientesService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Conexión a SQL Server
builder.Services.AddDbContext<ClientesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClientesBD")));

// Controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger siempre (puedes usar el if si prefieres solo en Development)
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();