using ClientesService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Conexión a SQL Server
builder.Services.AddDbContext<ClientesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClientesBD")));

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.Run();