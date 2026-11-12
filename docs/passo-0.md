| Ação no Swagger | Status que eu esperaria | Status que deu |
|---|---|---|
| Criar instituição | 201 | 400 (`"O id da instituição deve ser maior que zero."`) |
| Criar pet | 201 | 400 (`"O id do pet deve ser maior que zero."`) |
| Criar veterinário | 201 | 201 |
| Criar veterinário com email `email-invalido`, senha `1` e CPF `123` | 400 | 201 (gravou dados inválidos no banco sem validar) |
| Criar pet apontando para `institutionId: 999` (inexistente) | 400 | 400 (barrou antes no `"O id do pet deve ser maior que zero."`) |
| Listar funcionários logo depois de cadastrar um veterinário | só funcionários comuns | 200 (retornou os veterinários misturados na lista de `Employee`) |
| Apagar esse veterinário pela rota `DELETE /api/Employee/{id}` | 404 (não permitir apagar derivado pela rota base) | 204 (apagou o veterinário pela rota de `Employee`) |