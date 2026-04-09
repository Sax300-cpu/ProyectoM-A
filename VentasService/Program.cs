using Microsoft.EntityFrameworkCore;
using VentasService.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar el DbContext con la cadena de conexión
builder.Services.AddDbContext<VentasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VentasBD")));

// 2. Registrar AutoMapper (para mapear entre entidades y DTOs)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 3. Registrar controladores
builder.Services.AddControllers();

// 4. Registrar Swagger (para probar la API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Registrar HttpClient para llamadas a otros servicios
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// 5. Configuración del pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // habilita CORS

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
