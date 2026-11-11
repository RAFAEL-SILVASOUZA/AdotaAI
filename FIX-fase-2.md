# 🛠️ FIX — Fase 2 (Institution, Pet, Employee, Veterinarian, Attendant, Adm)

João, revisei as suas 6 verticais. Antes da lista: **tem coisa boa aqui.** O mapeamento de herança que você escolheu (TPT) saiu coerente do começo ao fim — configurations, migrations e snapshot batendo, FKs certas, sem `Discriminator` indevido. E você sacou sozinho o ponto mais sutil do TPT: os checks de email/CPF dos derivados consultando a hierarquia toda. Isso é nível sênior. Parabéns.

O problema: **a sua fase não rodou.** Os 59 testes que estão verdes no `dotnet test` são todos do User — nenhuma linha de teste das suas 6 verticais existe. E quando a gente não roda, a gente entrega o que tem aqui: dois endpoints de criação que falham 100% das vezes e um cluster inteiro que grava dados inválidos no banco.

Este documento **não dá a resposta**. Ele diz o que você vai reproduzir e por onde atacar, na ordem. A regra de ouro dele: **nenhum código é corrigido antes de virar um teste vermelho.** Se você consertar antes de testar, eu não vou conseguir diferenciar o que você aprendeu do que você memorizou.

> **Como vou acompanhar:** cada passo termina em commit com `dotnet build` e `dotnet test` verdes. Não vou olhar a solução — vou olhar os testes que você escreveu e a tabela do passo 0.

---

## Passo 0 — Reproduza tudo (antes de abrir o VS Code para "consertar")

Suba o projeto (`dotnet ef database update`, `dotnet run`) e chame no Swagger, com corpo válido, cada endpoint novo. Preencha esta tabela e faça o commit dela (`docs:`) — é a sua primeira entrega:

| Ação no Swagger | Status que eu esperaria | Status que deu |
|---|---|---|
| Criar instituição | 201 | ? |
| Criar pet | 201 | ? |
| Criar veterinário | 201 | ? |
| Criar veterinário com email `email-invalido`, senha `1` e CPF `123` | 400 | ? |
| Criar pet apontando para `institutionId: 999` (inexistente) | 400 | ? |
| Listar funcionários logo depois de cadastrar um veterinário | só funcionários comuns | ? |
| Apagar esse veterinário pela rota `DELETE /api/Employee/{id}` | ? | ? |

Não adiante a pesquisa. Se a tabela ficou confusa, **essa confusão é o objetivo do passo.**

## Passo 1 — Escreva os testes que faltam (ainda sem corrigir nada)

Copie o padrão do `tests/AdotaAI.Tests` (unit com fake em memória; integration com SQLite e `WebApplicationFactory`) para as 6 verticais. Para cada uma, no mínimo:

1. `Create` com corpo válido → 201 e o registro volta no GET;
2. `Create` com cada campo obrigatório inválido → 400 com a mensagem certa;
3. `Create` com email (e CPF/CRMV onde existir) duplicado → 400;
4. `GET`, `PUT` e `DELETE` de id inexistente → 404;
5. `Update` que não encontra → o contrato que você definiu, testado;
6. A resposta **nunca** contém a senha.

Rode `dotnet test`. Vários testes vão nascer vermelhos — inclusive os que falham **por bug do código, não do teste**. Deixe-os vermelhos no commit e escreva na mensagem quantos falham. É o seu relatório de bugs, executável.

## Passo 2 — Sintomas → por onde procurar (a resposta continua sendo sua)

| Sintoma (do passo 0) | Onde está a resposta |
|---|---|
| Criar instituição/pet retorna 400 com "o id deve ser maior que zero" | Releia a tabela de pegadinhas do README e compare o seu validator com o `UserValidator` **caracter a caracter**. A pegadinha está documentada neste repo desde sempre. |
| Veterinário entra com email inválido, senha de 1 caractere, CPF torto | Abra o `UserService` e o seu `VeterinarianService` lado a lado. O construtor de um recebe uma coisa que o outro não recebe — e é ela que chama o validator. Repita o exercício para os 4 do cluster. |
| Pet/veterinário aceitando instituição 999 | Compare o mapeamento do `Attendant` com o do `Veterinarian` e o do `Pet`: um deles está completo. Depois pergunte ao serviço: quem checa se a instituição existe **antes** de salvar? |
| Veterinário aparecendo como funcionário no `GET /api/Employee` | Em TPT, o `DbSet<Employee>` consulta a hierarquia inteira. Isso não é (só) um bug: pode ser decisão de design — mas decisão precisa ser **escolhida e registrada**, não descoberta por acidente. → Passo 3. |
| `DELETE /api/Employee/{id}` apagando um veterinário em cascata | Mesmo tema da linha acima. Rastreie o FK do `Id` nas tabelas derivadas. |
| `PUT /api/Employee/{id}` num veterinário respondendo 204 mas sem mudar nada | `SetValues` num derivado com uma entidade base atualiza só os campos da base. Silenciosamente. Teste isso. |
| 400 com "Dados do usuário..." em endpoints que não são de usuário | A mensagem está fixa num lugar só. Cada entidade merece a sua. |
| Mensagem da regra de `InstitutionId` dizendo "id do veterinário" | Copiei? Reaproveitei? Leia o `VeterinarianValidator` com olhos de usuário final. |
| Corrigi o validator de Id e agora os 4 do cluster deram 400 de uma vez | Parabéns, você encontrou a pegadinha do Id pela **terceira vez** (a vez que faltava estava no `EmployeeValidator`, escondida atrás do validator que nunca rodava). |
| Institution com `document`/`operatingHours` vazios vira 500 | A regra faltante não é do tamanho — é da existência. E 500 nunca deve acontecer por dado inválido; leia o fluxo do `InstitutionService`. |

Regra de ouro do passo: para cada sintoma consertado, o teste vermelho do passo 1 tem que nascer verde. Um de cada vez, um commit por correção.

## Passo 3 — Duas decisões que precisam de conversa (não programe sozinho)

1. **Hierarquia Employee×Veterinarian/Attendant/Adm na API.** Você tem três portas para o mesmo registro. Vamos decidir juntos: filtro por tipo exato ou hierarquia exposta de propósito — e o que cada uma faz com `PUT`/`DELETE`. Me traz 2 argumentos para cada lado.
2. **O que você decidiu e não registrou.** O exercício pedia: estratégia de herança **registrada no README** (você usou TPT — certo, só não contou); tamanho de `Document` e `OperatingHours` "diga ao professor o que escolheu"; `int` vs navegação no `InstitutionId`. Tudo ainda com checkbox desmarcado no TO-DO do README. Registre agora, com uma frase de justificativa em cada.

## Passo 4 — Detalhes para a passagem de vassoura (teste verde + commit em cada)

- [ ] Um valor do enum de status do pet está escrito errado — apareceu onde? No Swagger, no contrato da API. Corrija cedo ou carregue para sempre.
- [ ] Uma regra do `PetValidator` sobre `bool` é matematicamente incapaz de falhar — ache e elimine.
- [ ] Tem um `HasMaxLength` em coluna `INTEGER` pedindo caridade.
- [ ] Um primary constructor com `id` fora de posição — só não causou tragédia porque os serviços usam argumentos nomeados. Ajeite a ordem e o resto continua igual.
- [ ] Uma listagem por instituição sem `OrderBy` — as outras têm.
- [ ] Os ResponseDto do cluster repetem o mapeamento campo a campo 4 vezes; o padrão tem um `FromEntity` estático esperando cópia.
- [ ] Geração de migration nova (quando mexer nos FKs): **nunca edite migration já aplicada.** A `AddPetTable` foi editada à mão no seu passado e o histórico mentiu — não repita.

## Passo 5 — Processo (é isso que eu quero ver na próxima fase)

- **Commit por passo real.** O histórico mostra a vertical inteira (entidade → DTO → repo → controller) dentro do mesmo minuto, quatro vezes. Não preciso de 40 commits — preciso ver o build quebrar e consertar entre eles.
- Passos 8 e 10 do fluxo (testes + **chamar cada endpoint no Swagger** e conferir os cinco status) não são opcionais. Se tivessem sido feitos, os dois bugs críticos teriam morrido no nascimento.
- Manutenção pendente do TO-DO: a `.sln` para `dotnet test` funcionar na raiz.

---

**Ordem de ataque:** passo 0 → 1 → 2 → 3 → 4 → 5. Não leia o passo seguinte antes de terminar o atual. Se um sintoma parecer inexplicável por mais de uma tarde, me chama — inexplicável quase sempre é um `null` ou um namespace. Bons debuggings.
