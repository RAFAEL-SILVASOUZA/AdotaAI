using AdotaAI.Domain;
using AdotaAI.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(PetValidator).Assembly);

var app = builder.Build();

// TESTANDO

Console.WriteLine("\nTESTE USER INVÁLIDO");
var userInvalido = new User(
    id: 0,
    name: "A",
    email: "email-invalido",
    password: "123",
    phone: "123",
    address: "Rua",
    age: 19,
    photo: "",
    gender: (Sex)2 
);

var userValidator = new UserValidator();
var resUser = userValidator.Validate(userInvalido);

if (!resUser.IsValid)
{
    foreach (var error in resUser.Errors)
        Console.WriteLine($"[x] {error.PropertyName}: {error.ErrorMessage}");
}


Console.WriteLine("\nTESTE VETERINÁRIO INVÁLIDO");
var vetInvalido = new Veterinarian(
    id: 0,
    name: "Dr",
    email: "email-invalido",
    password: "123",
    cpf: "123",
    crmv: "12",
    institutionId: 0
);

var vetValidator = new VeterinarianValidator();
var resVet = vetValidator.Validate(vetInvalido);

if (!resVet.IsValid)
{
    foreach (var error in resVet.Errors)
        Console.WriteLine($"[x] {error.PropertyName}: {error.ErrorMessage}");
}


Console.WriteLine("\nTESTE USER VÁLIDO");
var userValido = new User(
    id: 1,
    name: "Maria Silva",
    email: "maria@email.com",
    password: "senhaSegura123",
    phone: "11999998888",
    address: "Rua das Flores, 123",
    age: 25,
    photo: "perfil.jpg",
    gender: Sex.Female
);

var resUserValido = userValidator.Validate(userValido);
if (resUserValido.IsValid)
{
    Console.WriteLine("[v] Usuário válido.");
}
Console.WriteLine("FIM\n");

app.MapGet("/", () => "Hello World!");

app.Run();