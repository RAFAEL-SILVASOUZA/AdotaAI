# AdotaAI

Bem-vindo, João! Este repositório é parte do curso. Este documento explica **o que já foi ajustado** no projeto, **como funcionam as validações** e **o que ainda falta para você fazer**.

Siga a ordem: leia o disclaimer, veja o que mudou, entenda a validação de `Pet` (que já está pronta como exemplo) e depois siga o TO-DO.

---

## ⚠️ Disclaimer

- Este projeto usa **.NET 8** e **FluentValidation 12.1.1**.
- **Atenção a uma pegadinha da v12:** o namespace de registro no DI mudou. As documentações online ainda mostram `using FluentValidation.DependencyInjectionExtensions;`, mas na v12 a extensão `AddValidatorsFromAssembly` está no namespace `FluentValidation` (classe `ServiceCollectionExtensions`). Usar o `using` antigo gera o erro `CS0234`.
- As classes do domínio usam **primary constructors** (parâmetros no cabeçalho da classe) e **propriedades `private set`**. Não mude isso sem necessidade.
- Os nomes de arquivos e pastas foram padronizados para **inglês** (ex.: `Atendente.cs` virou `Attendant.cs`). Mantenha esse padrão.
- **O que é DI (injeção de dependência)?** É um jeito de o próprio projeto "montar" os objetos para você. Em vez de você criar o validador com `new PetValidator()`, você pede para o `Program.cs` registrar ele (com `AddValidatorsFromAssembly`) e, quando precisar, o .NET entrega uma instância pronta. Assim você não se preocupa em criar e gerenciar os objetos manualmente.

---

## 🖥️ Como rodar o projeto no terminal

Antes de começar, você precisa ter o **.NET 8 SDK** instalado no seu computador. Para conferir, abra o terminal e digite:

```bash
dotnet --version
```

Se aparecer algo como `8.0.x`, está tudo certo. Se aparecer um erro, baixe o SDK em [dotnet.microsoft.com](https://dotnet.microsoft.com/download) e instale a versão 8.

### O que é o terminal?

É uma janela onde você digita comandos para o computador. No Windows, você pode usar o **PowerShell** (busque por "PowerShell" no menu Iniciar) ou o **Terminal** (busque por "Terminal"). No VS Code, dá para abrir o terminal de dentro mesmo: menu `Terminal` > `New Terminal`.

### Passo a passo

**1. Entre na pasta do projeto**

Abra o terminal e navegue até a pasta onde o projeto está. Se o projeto está em `D:\treinamento\AdotaAI`, digite:

```bash
cd D:\treinamento\AdotaAI
```

> `cd` significa "change directory" (mudar de pasta). Sempre que você abrir o terminal, precisa fazer isso para o computador saber onde o projeto está.

**2. Restaure os pacotes**

```bash
dotnet restore
```

> **O que isso faz?** O projeto usa "pacotes" (bibliotecas prontas de outras pessoas, como o FluentValidation). O `restore` baixa esses pacotes para o seu computador. Você só precisa rodar isso **na primeira vez** ou quando alguém adicionar um pacote novo. Se já restaurou antes, pode pular.

**3. Compile o projeto (build)**

```bash
dotnet build
```

> **O que isso faz?** O `build` transforma seu código C# em algo que o computador consegue executar. Se tudo estiver certo, vai aparecer `Build succeeded`. Se aparecer `Build FAILED`, tem algum erro no código e o terminal vai mostrar qual linha está com problema. **Sempre rode o `build` depois de mudar código** para saber se quebrou algo.

**4. Rode o projeto**

```bash
dotnet run
```

> **O que isso faz?** O `run` compila e já executa o projeto. Como este é um projeto web, ele vai abrir um servidor local. No terminal vai aparecer algo como `Now listening on: http://localhost:5000`. Abra esse endereço no navegador para ver se está funcionando.
>
> **Para parar o projeto**, volte no terminal e aperte `Ctrl + C`.

### Resumo rápido (para você decorar)

| Comando | O que faz | Quando usar |
|---------|-----------|-------------|
| `dotnet restore` | Baixa os pacotes | Primeira vez ou quando adicionar pacote novo |
| `dotnet build` | Compila o código | Sempre que mudar algo, para checar erros |
| `dotnet run` | Compila e executa | Quando quiser testar o projeto rodando |

> **Dica:** na maioria das vezes você só vai usar `dotnet build` (para checar se o código compila) e `dotnet run` (para testar). O `restore` é raro.

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
| `feat` | Uma funcionalidade **nova** | `feat: adicionar UserValidator` |
| `fix` | Uma **correção** de erro | `fix: corrigir mensagem de erro da idade` |
| `refactor` | Mudança **interna** que não altera o comportamento | `refactor: renomear arquivos do domínio para inglês` |
| `docs` | Mudança **só em documentos** (como este README) | `docs: atualizar o TO-DO` |
| `test` | Adição ou mudança de **testes** | `test: adicionar teste do PetValidator` |
| `chore` | Tarefas de **manutenção** (instalar pacote, limpar código) | `chore: instalar FluentValidation` |

### Regras simples

1. **Um commit = uma coisa só.** Não misture "adicionar validador" com "renomear arquivo" no mesmo commit. Se fez duas coisas, faça dois commits.
2. **Descrição curta.** No máximo uns 50 caracteres. Se precisar de mais detalhes, escreva na parte de baixo da mensagem (depois de uma linha em branco).
3. **Em português está tudo bem.** A mensagem é para você e para quem for ler o histórico depois.
4. **Commit com frequência.** Fez um passo do TO-DO? Commita. Assim, se algo der errado, você consegue voltar para um ponto anterior.

### Exemplo na prática

```bash
# 1. Veja o que mudou
git status

# 2. Adicione os arquivos que quer salvar
git add Validation/UserValidator.cs

# 3. Crie o commit com a mensagem no formato certo
git commit -m "feat: adicionar UserValidator"
```

> **Dica:** se você digitar `git commit` sem a mensagem `-m`, o Git vai abrir um editor para você escrever. Na primeira linha, escreva `tipo: descrição` e salve.

---

## O que foi ajustado (resumo do git)

### 1. Renomeação de arquivos (padronização para inglês)
| Antes | Depois |
|-------|--------|
| `Domain/Atendente.cs` | `Domain/Attendant.cs` |
| `Domain/Funcionario.cs` | `Domain/Employee.cs` |
| `Domain/Instituicao.cs` | `Domain/Institution.cs` |
| `Domain/Usuario.cs` | `Domain/User.cs` |
| `Domain/Veterinario.cs` | `Domain/Veterinarian.cs` |

### 2. Reorganização de pastas e enums
| Antes | Depois |
|-------|--------|
| `Domain/Pet info/` | `Domain/PetInfo/` |
| `Domain/Pet info/TamanhoPet.cs` | `Domain/PetInfo/Size.cs` |
| `Domain/User info/` | `Domain/UserInfo/` |

### 3. Refatoração das classes de domínio
Todas as classes (`Pet`, `User`, `Institution`, `Employee`, `Veterinarian`, `Attendant`, `Adm`) foram:
- Movidas para o namespace `AdotaAI.Domain`.
- Refatoradas para usar **primary constructors** (ex.: `public class Pet(string name, ...)`) em vez de atribuir cada propriedade no corpo do construtor.

### 4. Camada de validação (novo)
- Instalados os pacotes `FluentValidation` e `FluentValidation.DependencyInjectionExtensions` (v12.1.1) no `AdotaAI.csproj`.
- Criada a pasta `Validation/` com o `PetValidator.cs`.
- Registro dos validadores no `Program.cs` via `AddValidatorsFromAssembly`.

---

## Como funciona a validação (exemplo: Pet)

O validador fica em `Validation/PetValidator.cs`. Ele herda de `AbstractValidator<Pet>` e define uma regra por propriedade:

```csharp
public class PetValidator : AbstractValidator<Pet>
{
    public PetValidator()
    {
        RuleFor(pet => pet.Name)
            .NotEmpty().WithMessage("O nome do pet não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome do pet deve ter pelo menos 2 caracteres.")
            .MaximumLength(50).WithMessage("O nome do pet não pode ter mais de 50 caracteres.");

        RuleFor(pet => pet.Age)
            .InclusiveBetween(0, 30).WithMessage("A idade do pet deve estar entre 0 e 30 anos.");

        // ... demais regras
    }
}
```

### Regras aplicadas no `Pet`
| Propriedade | Regra |
|-------------|-------|
| `Name` | Obrigatório, 2 a 50 caracteres |
| `Race` | Obrigatória, 2 a 50 caracteres |
| `Age` | Entre 0 e 30 |
| `Photo` | Obrigatório |
| `VetRecord` | Obrigatório |
| `BehaviourDesc` | Obrigatório, máx. 500 caracteres |
| `Id` | Maior que zero |

### Registro no DI (`Program.cs`)
```csharp
using AdotaAI.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(PetValidator).Assembly);

var app = builder.Build();
```

> O `AddValidatorsFromAssembly` varre o assembly e registra **todos** os validadores que você criar na pasta `Validation/`. Por isso, ao criar um validador novo, ele já fica disponível no DI automaticamente, sem precisar registrar um a um.

### Como usar um validador
```csharp
var validator = new PetValidator();
var result = validator.Validate(pet);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

---

## ✅ TO-DO para o João

Siga nesta ordem. A validação de `Pet` já está pronta e serve de modelo para as demais.

- [ ] **1. Entender o exemplo**
  - Leia a seção "Como rodar o projeto no terminal" (logo acima) e siga o passo a passo.
  - Leia `Validation/PetValidator.cs` e `Program.cs`.
  - Rode `dotnet build` e confirme que aparece `Build succeeded`.

- [ ] **2. Criar o `UserValidator`** (`Validation/UserValidator.cs`)
  - `Name`: obrigatório, 2 a 50 caracteres.
  - `Email`: obrigatório, formato de e-mail válido (`EmailAddress()`).
  - `Phone`: obrigatório, 10 a 15 caracteres.
  - `Age`: entre 18 e 100.
  - `Password`: obrigatório, mínimo de 6 caracteres.
  - `Address`: obrigatório.
  - `Id`: maior que zero.

- [ ] **3. Criar o `InstitutionValidator`** (`Validation/InstitutionValidator.cs`)
  - `Name`: obrigatório, 2 a 100 caracteres.
  - `Email`: obrigatório, formato de e-mail válido.
  - `Phone`: obrigatório.
  - `Address`: obrigatório.
  - `Password`: obrigatório, mínimo de 6 caracteres.
  - `Description`: obrigatório, máx. 500 caracteres.
  - `Id`: maior que zero.

- [ ] **4. Criar o `EmployeeValidator`** (`Validation/EmployeeValidator.cs`)
  - `Name`: obrigatório, 2 a 100 caracteres.
  - `Email`: obrigatório, formato de e-mail válido.
  - `Password`: obrigatório, mínimo de 6 caracteres.
  - `CPF`: obrigatório, 11 dígitos.
  - `Id`: maior que zero.

- [ ] **5. Criar o `VeterinarianValidator`** (`Validation/VeterinarianValidator.cs`)
  - Herde as regras de `Employee` (ou use `Include(new EmployeeValidator())`).
  - `Crmv`: obrigatório, 5 a 10 caracteres.
  - `InstitutionId`: maior que zero.

- [ ] **6. Criar o `AttendantValidator`** (`Validation/AttendantValidator.cs`)
  - Herde as regras de `Employee` (ou use `Include(new EmployeeValidator())`).
  - `InstitutionId`: maior que zero.

- [ ] **7. Criar o `AdmValidator`** (`Validation/AdmValidator.cs`)
  - Herde as regras de `Employee` (ou use `Include(new EmployeeValidator())`).

- [ ] **8. Testar os validadores**
  - Crie uma classe de teste ou um endpoint temporário para validar cada entidade.
  - Confirme que mensagens de erro aparecem para dados inválidos.

- [ ] **9. Revisão final**
  - Rode `dotnet build` e garanta que não há erros.
  - Confira se todos os validadores estão na pasta `Validation/` (o registro no DI é automático).

---

## Dicas

- **Dica de reuso:** para `Veterinarian`, `Attendant` e `Adm` (que herdam de `Employee`), use `Include(new EmployeeValidator())` no construtor do validador para reaproveitar as regras do pai e só adicionar as regras específicas.
- **Dica de mensagens:** sempre use `.WithMessage(...)` com texto claro em português, para o aluno final entender o que está errado.
- **Dica de enum:** para propriedades do tipo `enum` (ex.: `Species`, `Size`, `Status`), você pode usar `.IsInEnum()` para garantir que o valor é válido.
