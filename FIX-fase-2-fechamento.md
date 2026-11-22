# 🛠️ FIX: Fechamento da Fase 2 (3 pendências + 1 decisão)

> **Nota de estilo:** este documento dispensa o uso do caractere "—" (travessão). A convenção do curso é pontuação simples: vírgula, ponto e vírgula ou ponto final no lugar.

João, parabéns pela fase 2. O mapeamento TPT foi coerente do começo ao fim, os 321 testes estão verdes e o `.sln` na raiz faz o `dotnet test` funcionar sem caminho. O passo 0 foi feito com honestidade: a tabela mostra o "antes", sem filtro. Isso é o que eu quero ver em todo o curso.

Este documento **não dá a resposta**. Ele diz o que você vai fazer, na ordem. E uma decisão que não é sua de tomar. **Nada aqui é um bug de fundo**: são detalhes que aparecem no dia a dia de quem faz API de verdade.

> **Como vou acompanhar:** cada passo termina em commit com `dotnet build` e `dotnet test` verdes. Não vou olhar a solução. Vou olhar os testes que você escreveu e a tabela do passo 0.

---

## Passo 0: Re-execute a tabela do passo 0 (antes de mexer em qualquer coisa)

Suba o projeto com `dotnet ef database update` (banco seu, não o do professor) e chame no Swagger, com corpo válido, cada endpoint: **da mesma forma que no passo 0, agora com as correções vivas**. Preencha a tabela com o status "depois" e faça o commit dela (`docs:`). É a sua primeira entrega:

| Ação no Swagger | Status esperado (novo) | Status que deu |
|---|---|---|
| Criar instituição | 201 | ? |
| Criar pet | 201 | ? |
| Criar veterinário | 201 | ? |
| Criar veterinário com email `email-invalido`, senha `1` e CPF `123` | 400 | ? |
| Criar pet (e veterinário) com `institutionId: 999` | 400 com "Instituição informada não encontrada." | ? |
| Listar funcionários logo depois de cadastrar um veterinário | hierarquia **conforme a decisão do passo 2** | ? |
| Apagar esse veterinário pela rota `DELETE /api/Employee/{id}` | conforme a decisão do passo 2 | ? |

## Passo 1: Mensagem por entidade (a única correção de código obrigatória)

Releia `Application/Exceptions/ValidationFailedException.cs`. O construtor aceita um parâmetro opcional `message`. **E ninguém o usa:** toda vertical responde ainda com `"Dados do usuário são inválidos."`.

O caminho: cada serviço passar a mensagem do seu domínio (ex: `"Dados do pet são inválidos."`). **Regra de ouro: nenhum código é corrigido antes de virar um teste vermelho.** Escreva o teste que exige a mensagem certa, veja ele falhar, corrija, veja ele nascer verde, commit.

Não esqueça do `Institution` e do `Pet`: eles têm `ValidationFailedException` nas mensagens específicas de duplicidade e de instituição inexistente. A mensagem fixa não deveria vazar em nenhuma das verticais.

## Passo 2: Cobertura de `Pet` e `Institution`

Comparando os tamanhos (e a lista de cenários) das suas 6 verticais, `Pet` e `Institution` ficam abaixo das outras. Cada vertical nova deve ter:

- criação com corpo válido → 201 e o registro volta no GET;
- criação com cada campo obrigatório inválido → 400 com as mensagens certas;
- `GET`/`PUT`/`DELETE` de id inexistente → 404;
- **`PUT` e `DELETE` válidos** → 204;
- `GET` existente e `GET` listagem → 200.

Não precisa inventar mais do que isso: complete o que falta.

## Passo 3: A decisão de hierarquia (não execute sozinho)

O FIX da fase 2 deixou isso como decisão que precisa da gente: **o que `/api/Employee` faz com as subclasses?** (hierarquia exposta de propósito vs. filtro por tipo exato; e o que cada uma faz com `PUT`/`DELETE`.)

João, o README **registrou** sua decisão pessoal. Isso ainda não é a decisão final. É a sua recomendação. O passo:

1. Me traz **2 argumentos para cada lado** (por escrito, em `docs/hierarquia.md` ou na conversa).
2. Decido junto com você.
3. **Registremos a decisão tomada** no README. Com a justificativa final.

Só depois disso é que você mexe em código.

## Passo 4: Processo

- Commit por passo real.
- Passos 0 e 7 (chamar cada endpoint no Swagger e conferir os cinco status) não são opcionais. Foram a lição da fase 2.
- Rode `dotnet test` na raiz (o `.sln` está pronto) e garanta que nada quebrou.

---

**Ordem de ataque:** passo 0 → 1 → 2 → 3 → 4. Não leia o passo seguinte antes de terminar o atual. Se um sintoma parecer inexplicável por mais de uma tarde, me chama. Inexplicável quase sempre é um `null` ou um namespace. Boa escrita.
