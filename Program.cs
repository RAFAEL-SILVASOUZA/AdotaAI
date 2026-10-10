using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using AdotaAI.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DI: DbContext (SQLite)
builder.Services.AddDbContext<AdotaAIDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AdotaAIDb")));

// DI: validadores (varre o assembly e registra todos os validators)
builder.Services.AddValidatorsFromAssembly(typeof(UserValidator).Assembly);

// DI: repositório e serviço da vertical User
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AdotaAI.Application.UserService>();

builder.Services.AddControllers();

// Swagger: explorador de endpoints e interface para chamar os endpoints.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AdotaAI",
        Version = "v1",
        Description = "API do curso: verticals de User, Pet, Institution, Employee, Veterinarian, Attendant e Adm."
    }));

var app = builder.Build();

// Swagger só em Development: em produção a API não expõe o explorador.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Tratamento de erros da camada Application (controller permanece fino).
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (AdotaAI.Application.Exceptions.ValidationFailedException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message, errors = ex.Errors });
    }
    catch (AdotaAI.Application.Exceptions.NotFoundException ex)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    }
});

app.MapControllers();

app.Run();

// Permite que WebApplicationFactory (projeto de testes) referencie o entry point.
public partial class Program
{
}
