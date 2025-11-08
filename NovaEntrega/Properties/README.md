# Properties — launchSettings.jsonguia 

Este arquivo controla como a API roda no ambiente de desenvolvimento.
Como aluno, pense nele como “configuração de execução local”.

Principais pontos:
- `profiles`: definem jeitos diferentes de iniciar a API (http/https).
- `applicationUrl`: as URLs e portas onde a API vai escutar.
- `ASPNETCORE_ENVIRONMENT`: normalmente fica `Development` quando roda local.

Perfis existentes (padrão do projeto):
- `http`: usa `http://localhost:5000`
- `https`: usa `https://localhost:5001`

Como rodar escolhendo o perfil:
```bash
dotnet run --launch-profile http
```

Trocar portas (opcional):
- Abra `Properties/launchSettings.json`
- Ajuste o valor de `applicationUrl` do perfil desejado
- Ex.: `"applicationUrl": "http://localhost:5100"`

Dica:
- Se o Swagger não abrir, confirme se você está usando o perfil certo
  e se a porta não está ocupada por outro processo.