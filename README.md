# AdotaAI

Bem-vindo, João! Este repositório é parte do curso. Este documento explica **o que já foi feito**, **como funciona a vertical do `User`** (que é o modelo a copiar) e **o que falta você fazer** para as outras entidades.

Siga a ordem: leia o disclaimer, veja o que mudou, entenda a vertical do `User` arquivo por arquivo, leia as pegadinhas e depois siga o TO-DO.

> A versão anterior deste README (a fase das validações) está guardada em `backup/README.md`. Ela não foi apagada: o que ela pedia já está feito.

---

## ⚠️ Disclaimer

- Este projeto usa **.NET 8**, **FluentValidation 12.1.1** e **Entity Framework Core 8.0.10** com **SQLite**.
- **Atenção a uma pegadinha da v12:** o namespace de registro no DI mudou. As documentações online ainda mostram `using FluentValidation.DependencyInjectionExtensions;`, mas na v12 a extensão `AddValidatorsFromAssembly` está no namespace `FluentValidation` (classe `ServiceCollectionExtensions`). Usar o `using` antigo gera o erro `CS0234`.
- As classes do domínio usam **primary constructors** (parâmetros no cabeçalho da classe) e **propriedades `private set`**. Não mude isso sem necessidade.
- Os nomes de arquivos e pastas foram padronizados para **inglês** (ex.: `Atendente.cs` virou `Attendant.cs`). Mantenha esse padrão. **Mensagens de erro e comentários ficam em português.**
- O banco é um arquivo `adotaai.db` na raiz. Ele está no `.gitignore` (`adotaai.db`, `adotaai.db-shm`, `adotaai.db-wal`): cada pessoa gera o seu com `dotnet ef database update`.
- **O que é DI (injeção de dependência)?** É um jeito de o próprio projeto "montar" os objetos para você. Em vez de você criar o validador com `new PetValidator()`, você pede para o `Program.cs` registrar ele, e quando precisar o .NET entrega uma instância pronta. Você não se preocupa em criar e gerenciar os objetos manualmente.
- **O que é uma vertical?** É o caminho completo de uma entidade: da entrada HTTP até o banco. Uma vertical tem 7 peças, sempre nos mesmos lugares. Você faz uma por vez, termina, commita, e só então começa a próxima.

---

## 🖥️ Como rodar o projeto no terminal

Antes de começar, você precisa ter o **.NET 8 SDK** instalado. Confira:

```bash
dotnet --version
```

Se aparecer algo como `8.0.x`, está tudo certo. Se aparecer um erro, baixe o SDK em [dotnet.microsoft.com](https://dotnet.microsoft.com/download) e instale a versão 8.

### O que é o terminal?

É uma janela onde você digita comandos para o computador. No Windows, você pode usar o **PowerShell** (busque por "PowerShell" no menu Iniciar) ou o **Terminal** (busque por "Terminal"). No VS Code, dá para abrir o terminal de dentro mesmo: menu `Terminal` > `New Terminal`.

### Passo a passo

**1. Entre na pasta do projeto**

```bash
cd D:\treinamento\AdotaAI
```

> `cd` significa "change directory" (mudar de pasta). Sempre que você abrir o terminal, precisa fazer isso para o computador saber onde o projeto está.

**2. Restaure os pacotes**

```bash
dotnet restore
```

> **O que isso faz?** O projeto usa "pacotes" (bibliotecas prontas, como FluentValidation e EF Core). O `restore` baixa esses pacotes. Você só precisa rodar isso **na primeira vez** ou quando alguém adicionar um pacote novo.

**3. Crie o banco (só na primeira vez, ou quando aparecer migration nova)**

```bash
dotnet ef database update
```

> **O que isso faz?** Lê as migrations em `Infrastructure/Data/Migrations/` e cria a tabela `Users` no arquivo `adotaai.db`. Se `dotnet ef` não for conhecido, instale: `dotnet tool install --global dotnet-ef`.

**4. Compile o projeto (build)**

```bash
dotnet build
```

> **O que isso faz?** Transforma seu código C# em algo que o computador consegue executar. Se tudo estiver certo, vai aparecer `Build succeeded`. Se aparecer `Build FAILED`, tem algum erro no código e o terminal mostra qual linha está com problema. **Sempre rode o `build` depois de mudar código.**

**5. Rode o projeto**

```bash
dotnet run
```

> **O que isso faz?** Compila e executa. Como este é um projeto web, ele abre um servidor local. No terminal vai aparecer algo como `Now listening on: http://localhost:5000`. **Para parar**, aperte `Ctrl + C`.

**6. Rode os testes**

```bash
dotnet test tests/AdotaAI.Tests/AdotaAI.Tests.csproj
```

> **O que isso faz?** Executa os testes do projeto separado `tests/AdotaAI.Tests`. O caminho do `.csproj` é necessário porque ainda não existe arquivo `.sln` ligando os dois projetos. Para rodar só um grupo: `dotnet test --filter "FullyQualifiedName~Tests.Unit"` ou `--filter "FullyQualifiedName~Tests.Integration"`.

### Resumo rápido (para você decorar)

| Comando | O que faz | Quando usar |
|---------|-----------|-------------|
| `dotnet restore` | Baixa os pacotes | Primeira vez ou quando adicionar pacote novo |
| `dotnet ef migrations add Nome` | Gera a migration das mudanças no modelo | Depois de mudar entidade ou configuration |
| `dotnet ef database update` | Aplica as migrations no SQLite | Depois de gerar migration nova |
| `dotnet build` | Compila o código | Sempre que mudar algo, para checar erros |
| `dotnet run` | Compila e executa | Quando quiser testar o projeto rodando |
| `dotnet test` | Roda os testes | Sempre que mudar código de uma vertical |

> **Dica:** na maioria das vezes você só vai usar `dotnet build` e `dotnet run`. O `restore` é raro.

---

## 📝 Como fazer commits (Conventional Commits)

Você vai usar o Git para salvar o progresso do trabalho. Cada "commit" é como um **ponto de salvamento** com uma mensagem explicando o que mudou. Para manter o histórico organizado e fácil de ler, usamos o padrão **Conventional Commits**.

### Formato da mensagem

```
tipo: descrição curta do que foi feito
```

- **tipo**: uma palavra que diz *que tipo* de mudança foi. Sempre minúscula.
- **descrição**: curta, em linguagem simples, sem ponto final. Comece com verbo ("adicionar", "corrigir", "renomear").

### Tipos permitidos

| Tipo | Quando usar | Exemplo |
|------|-------------|---------|
| `feat` | Uma funcionalidade **nova** | `feat: adicionar PetRepository` |
| `fix` | Uma **correção** de erro | `fix: corrigir mensagem de erro da idade` |
| `refactor` | Mudança **interna** que não altera o comportamento | `refactor: renomear arquivos do domínio para inglês` |
| `docs` | Mudança **só em documentos** (como este README) | `docs: atualizar o TO-DO` |
| `test` | Adição ou mudança de **testes** | `test: adicionar testes da vertical Pet` |
| `chore` | Tarefas de **manutenção** (instalar pacote, limpar código) | `chore: criar solution do projeto` |

### Regras simples

1. **Um commit = uma coisa só.** Não misture "adicionar repositório" com "registrar DI" no mesmo commit. Se fez duas coisas, faça dois commits.
2. **Descrição curta.** No máximo uns 50 caracteres. Se precisar de mais detalhes, escreva na parte de baixo da mensagem (depois de uma linha em branco).
3. **Em português está tudo bem.** A mensagem é para você e para quem for ler o histórico depois.
4. **Commit com frequência.** Fez um passo do TO-DO? Commita. Assim, se algo der errado, você consegue voltar para um ponto anterior.

### Exemplo na prática

```bash
# 1. Veja o que mudou
git status

# 2. Adicione os arquivos que quer salvar
git add Repositories/PetRepository.cs

# 3. Crie o commit com a mensagem no formato certo
git commit -m "feat: adicionar PetRepository com Entity Framework"
```

> **Dica:** se você digitar `git commit` sem a mensagem `-m`, o Git vai abrir um editor para você escrever. Na primeira linha, escreva `tipo: descrição` e salve.

---

## ✅ O que já está pronto

### Fase 1: validações (todas as entidades)

| Arquivo | O que faz |
|---------|-----------|
| `Validation/PetValidator.cs` | Regras de `Pet` |
| `Validation/UserValidator.cs` | Regras de `User` |
| `Validation/InstitutionValidator.cs` | Regras de `Institution` |
| `Validation/EmployeeValidator.cs` | Regras de `Employee` |
| `Validation/VeterinarianValidator.cs` | `Include(new EmployeeValidator())` + `Crmv` |
| `Validation/AttendantValidator.cs` | `Include(new EmployeeValidator())` + `InstitutionId` |
| `Validation/AdmValidator.cs` | `Include(new EmployeeValidator())` |

O registro no DI varre o assembly, então **todo validador novo aparece automaticamente**, sem registrar um a um.

### Fase 2: vertical do `User` (o modelo)

| Camada | Arquivo | O que faz |
|--------|---------|-----------|
| Domain | `Domain/IUserRepository.cs` | Interface: o contrato, sem dependência de EF |
| Infrastructure | `Infrastructure/Data/AdotaAIDbContext.cs` | O `DbContext` com `DbSet<User>` |
| Infrastructure | `Infrastructure/Data/Configuration/UserEntityFrameworkConfiguration.cs` | Mapeamento: tabela, tamanhos, índice único |
| Infrastructure | `Infrastructure/Data/AdotaAIDbContextFactory.cs` | Factory de design-time para o `dotnet ef` |
| Infrastructure | `Infrastructure/Data/Migrations/` | Migration `InitialCreate` + snapshot |
| Repositories | `Repositories/UserRepository.cs` | Implementação do contrato usando EF |
| Application | `Application/Dto/CreateUserDto.cs`, `UpdateUserDto.cs`, `UserResponseDto.cs` | Entrada e saída da camada |
| Application | `Application/UserService.cs` | Casos de uso: validar, checar email, chamar repositório |
| Application | `Application/Exceptions/ValidationFailedException.cs`, `NotFoundException.cs` | Erros da camada |
| Controllers | `Controllers/UserController.cs` | Fino: só chama o serviço e devolve status |
| Raiz | `Program.cs` | DI + middleware de erro (400/404) |
| Raiz | `appsettings.json` | `ConnectionStrings:AdotaAIDb` |
| Testes | `tests/AdotaAI.Tests/Unit/` | 37 testes da validator e do serviço |
| Testes | `tests/AdotaAI.Tests/Integration/` | 22 testes do repositório e dos endpoints |

---

## 🧭 Como funciona a vertical (modelo para copiar)

A regra de ouro: **cada camada só conhece a de baixo.** O controller não conhece EF, o serviço não conhece HTTP, o repositório é o único que fala com o banco.

### 1. Contrato no Domain

```csharp
namespace AdotaAI.Domain;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> UpdateAsync(User user, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
```

> **Por que interface?** Porque o serviço depende do contrato, não do banco. Isso permite testar o serviço com um repositório falso em memória, sem SQLite.

### 2. Mapeamento no Infrastructure

```csharp
public class UserEntityFrameworkConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedOnAdd();
        builder.Property(user => user.Name).IsRequired().HasMaxLength(70);
        builder.HasIndex(user => user.Email).IsUnique();
    }
}
```

O `DbContext` aplica **todas** as configurations do assembly de uma vez:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdotaAIDbContext).Assembly);
```

> **O que isso significa para você?** Para uma entidade nova, o `OnModelCreating` do `DbContext` **não muda**. Você só cria o arquivo de configuration na pasta `Configuration/` e adiciona uma linha de `DbSet` no `DbContext`.

### 3. Implementação no Repositories

```csharp
public class UserRepository(AdotaAIDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Users.FindAsync([id], ct);
    }
}
```

> `FindAsync` procura pela chave primária. `AsNoTracking()` na listagem evita que o EF rastreie as entidades que só vão ser lidas.

### 4. DTOs na Application

```csharp
public record CreateUserDto(string Name, string Email, string Phone, int Age,
    string Photo, string Password, Sex Gender, string Address);

public record UserResponseDto(int Id, string Name, string Email, string Phone,
    int Age, string Photo, Sex Gender, string Address)
{
    public static UserResponseDto FromEntity(User user) =>
        new(user.Id, user.Name, user.Email, user.Phone, user.Age, user.Photo, user.Gender, user.Address);
}
```

> **Repare:** `UserResponseDto` **não tem `Password`**. A senha nunca sai da Application. Use `record` para DTOs: são dados, não comportamento.

### 5. Serviço de caso de uso

```csharp
public class UserService(IUserRepository repository, IValidator<User> validator)
{
    public async Task<int> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var user = new User(id: 0, name: dto.Name, /* ... */);

        await ValidateAsync(user, ct);

        if (await repository.EmailExistsAsync(dto.Email, ct))
            throw new ValidationFailedException(["O email informado já está cadastrado."]);

        await repository.AddAsync(user, ct);
        return user.Id;
    }
}
```

> **`id: 0` no create:** o banco gera o id. O serviço cria a entidade sem id, o EF preenche no `SaveChanges`, e o validator não reclama do zero.

### 6. Controller fino

```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController(UserService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken ct)
    {
        var id = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}
```

> **Fino** significa: nenhuma regra aqui. O controller converte HTTP em chamada de serviço e devolve o status (`201` criado, `200` ok, `204` deletado/atualizado). Os erros viram `400`/`404` no middleware do `Program.cs`.

### 7. Registro no Program.cs

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AdotaAI.Application.UserService>();
builder.Services.AddControllers();
```

> `Scoped` = uma instância por requisição. É o tempo de vida certo para `DbContext` e repositório.

---

## 🪰 Pegadinhas que já apareceram

Estas são as que doeu descobrir. Leia antes de começar, elas valem para todas as verticais.

| Pegadinha | O que acontece | Como resolver |
|-----------|----------------|---------------|
| Nome `ValidationException` | CS0104: colide com `FluentValidation.ValidationException` | Nomeie `ValidationFailedException` |
| Regra `Id > 0` no create | POST retorna 400 porque o id ainda é 0 | `.When(user => user.Id != 0)` |
| Glob do SDK Web | Os arquivos de `tests/` entram no build do projeto principal | `<Compile Remove="tests/**" />` no `AdotaAI.csproj` |
| `IDesignTimeDbContextFactory` | CS0246 sem o using | `using Microsoft.EntityFrameworkCore.Design;` |
| `ValidationResult` | CS0246 em testes | `using FluentValidation.Results;` |
| Pooling do SQLite | Arquivo de teste fica travado no `Dispose` | `Data Source=...;Pooling=False` |
| Environment em teste | Em Development a developer exception page responde 500 | `builder.UseEnvironment("Production")` no `WebApplicationFactory` |
| `AUTOINCREMENT` | O id não volta a 1 depois de `DELETE` | Não asserter `Assert.Equal(1, id)`; asserter `id > 0` e a ordem crescente |
| `dotnet test` na raiz | Não encontra o projeto de testes sem `.sln` | Passe o caminho do `.csproj` |

---

## ✅ TO-DO para o João

Uma vertical por vez, na ordem abaixo. Cada vertical segue os mesmos 7 passos. **Rode `dotnet build` e `dotnet test` ao final de cada uma e commita.**

> **Ordem dos passos em cada vertical**
> 1. `Domain/IXRepository.cs` (contrato)
> 2. `Infrastructure/Data/Configuration/XEntityFrameworkConfiguration.cs` + `DbSet` no `DbContext`
> 3. `dotnet ef migrations add NomeDaMigration` e `dotnet ef database update`
> 4. `Repositories/XRepository.cs`
> 5. `Application/Dto/` (Create, Update, Response)
> 6. `Application/XService.cs`
> 7. `Controllers/XController.cs` + registro no `Program.cs`
> 8. Testes unitários e integrados
> 9. `dotnet build`, `dotnet test`, commit

- [ ] **1. Vertical `Institution`**
  - Comece por ela: `Veterinarian` e `Attendant` têm `InstitutionId`, então a tabela de instituições precisa existir primeiro.
  - Properties: `Id`, `Name`, `Email`, `Phone`, `Address`, `Photo`, `Password`, `Description`, `Document`, `OperatingHours`.
  - Índice único em `Email` (mesmo padrão do `User`).
  - `Document` (CNPJ) e `OperatingHours`: defina o tamanho no mapeamento e diga ao professor o que escolheu.

- [ ] **2. Vertical `Pet`**
  - Properties: `Id`, `Name`, `Species`, `Race`, `Age`, `IsFemale`, `PetSize`, `Photo`, `VetRecord`, `BehaviourDesc`, `PetStatus`.
  - Três enums (`Species`, `Size`, `Status`): ficam guardados como `int`, mesmo padrão do `Sex` no `User`.
  - `PetValidator` tem `RuleFor(pet => pet.Id).GreaterThan(0)`: aplique `.When(pet => pet.Id != 0)` como foi feito no `UserValidator`.

- [ ] **3. Vertical `Employee`**
  - Properties: `Id`, `Name`, `Email`, `Password`, `CPF`.
  - **Decisão obrigatória:** `Veterinarian`, `Attendant` e `Adm` herdam de `Employee`. O EF precisa de uma estratégia para herança (TPH: uma tabela com tudo, ou TPT: uma tabela por tipo). **Pergunte ao professor qual usar antes de escrever o mapeamento** e registre a resposta no README.

- [ ] **4. Vertical `Veterinarian`**
  - Properties próprias: `Crmv`, `InstitutionId`.
  - `InstitutionId` é referência: decide se você cria `IInstitutionRepository` como navegação ou guarda só o `int`. Alinhe com o professor.

- [ ] **5. Vertical `Attendant`**
  - Property própria: `InstitutionId`. Mesmas decisões da vertical `Veterinarian`.

- [ ] **6. Vertical `Adm`**
  - Nenhuma property própria: só as de `Employee`.

- [ ] **7. Manutenção**
  - `chore: criar solution do projeto` (arquivo `.sln` ligando `AdotaAI` e `tests/AdotaAI.Tests`), para `dotnet test` funcionar na raiz.
  - Rode `dotnet build` e `dotnet test` e garanta que nada quebrou.

---

## Dicas

- **Dica de reuso:** o `AdotaAIDbContext` não precisa mudar quando você cria uma vertical. Só o `DbSet` e o arquivo de configuration.
- **Dica de migration:** rode `dotnet ef migrations add Nome` com o projeto parado. Nomeie a migration pelo que ela faz (`AddPetTable`, não `Migration1`).
- **Dica de mensagens:** sempre use `.WithMessage(...)` com texto claro em português, para o aluno final entender o que está errado.
- **Dica de enum:** para propriedades `enum`, use `.IsInEnum()` no validador e `builder.Property(...).IsRequired()` no mapeamento.
- **Dica de teste:** copie o padrão de `tests/AdotaAI.Tests`. Unit: serviço com fake em memória. Integration: repositório no SQLite real e endpoints com `WebApplicationFactory`. Cada teste começa com a tabela limpa.
- **Dica de teste de id:** o SQLite não zera o contador de ids. Teste que o id foi gerado (`> 0`) e que a lista vem em ordem crescente, nunca que o primeiro id é `1`.
