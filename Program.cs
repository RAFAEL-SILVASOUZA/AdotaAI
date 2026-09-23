using AdotaAI.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(PetValidator).Assembly);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
