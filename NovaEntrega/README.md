# Servidor_PI — API .NET 8 com SQLite (guia simples)

API para gestão de doações usando ASP.NET Core, EF Core e SQLite.
Foco em rodar local fácil, testar via Swagger e opcionalmente publicar no Azure.

## 📋 Requisitos

- .NET 8 SDK
- Visual Studio 2022 ou VS Code
- Azure Account (para deploy)

## 🚀 Como rodar local

1. Clone o repositório:
```bash
git clone <seu-repositorio>
cd NovaEntrega
```

2. Restaure os pacotes NuGet:
```bash
dotnet restore
```

3. Execute a aplicação (o banco será criado/migrado automaticamente):
```bash
dotnet run
```

Observação: na primeira execução, a aplicação garante a criação do banco.
Ela pode aplicar migrations disponíveis e/ou um script inicial (quando configurado).
Você não precisa rodar migrations manualmente para começar.

4. Acesse os serviços:
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/api/health
- **API Base**: http://localhost:5000/api

## ✅ O que já funciona

Os seguintes componentes foram testados e estão funcionando corretamente:

- ✅ **Health Check**: `GET /api/health` retorna `{"status":"ok"}`
- ✅ **API de Usuários**: Listagem e operações CRUD funcionando
- ✅ **Swagger UI**: Documentação interativa disponível
- ✅ **Banco de Dados SQLite**: Criado automaticamente na primeira execução
- ✅ **Logs**: mensagens no console durante execução
- ✅ **CORS**: Configurado para permitir requisições de qualquer origem

## 📚 Endpoints principais

### Health Check
- `GET /api/health` - Status da API

### Usuários
- `GET /api/usuarios` - Lista todos (com paginação: `?page=1&pageSize=10`)
- `GET /api/usuarios/{id}` - Busca por ID
- `GET /api/usuarios/publicar` - Publica todos os usuários
- `POST /api/usuarios` - Cria novo usuário
- `PUT /api/usuarios/{id}` - Atualiza usuário
- `DELETE /api/usuarios/{id}` - Deleta usuário

### Campanhas
- `GET /api/campanhas` - Lista todas
- `GET /api/campanhas/{id}` - Busca por ID
- `GET /api/campanhas/publicar` - Publica todas as campanhas
- `POST /api/campanhas` - Cria nova campanha
- `PUT /api/campanhas/{id}` - Atualiza campanha
- `DELETE /api/campanhas/{id}` - Deleta campanha

### Doações
- `GET /api/doacoes` - Lista todas
- `GET /api/doacoes/{id}` - Busca por ID
- `GET /api/doacoes/publicar` - Publica todas as doações
- `POST /api/doacoes` - Cria nova doação
- `PUT /api/doacoes/{id}` - Atualiza doação
- `DELETE /api/doacoes/{id}` - Deleta doação

### Notícias
- `GET /api/noticias` - Lista todas
- `GET /api/noticias/{id}` - Busca por ID
- `GET /api/noticias/publicar` - Publica todas as notícias
- `POST /api/noticias` - Cria nova notícia
- `PUT /api/noticias/{id}` - Atualiza notícia
- `DELETE /api/noticias/{id}` - Deleta notícia

### Relatórios
- `GET /api/relatorios` - Lista todos
- `GET /api/relatorios/{id}` - Busca por ID
- `GET /api/relatorios/publicar` - Publica todos os relatórios
- `POST /api/relatorios` - Cria novo relatório
- `PUT /api/relatorios/{id}` - Atualiza relatório
- `DELETE /api/relatorios/{id}` - Deleta relatório

### Views
- `GET /api/views/buscar-nome?usuario=xxx` - Busca nome completo e nome de usuário
- `GET /api/views/doacoes-detalhadas` - Lista doações com detalhes de usuário e campanha

## 🗄️ Banco de dados

- **Tipo**: SQLite
- **Local dev**: `Data/app.db`
- **Produção (Azure)**: `D:\home\site\wwwroot\Data\app.db`
- **Criação**: automática na primeira execução

### Como a criação acontece

Ao iniciar, a API verifica se o banco existe e aplica o esquema necessário.
Se houver migrations, elas são aplicadas. Caso exista script inicial configurado,
ele pode ser usado na primeira criação. Para você, basta rodar `dotnet run`.

### Dados de teste

O banco já vem com dados de teste pré-inseridos para facilitar os testes:

- **2 Usuários**: João Silva Santos (`joao123`) e Maria Oliveira (`maria456`)
- **2 Campanhas**: "Campanha do Agasalho 2024" e "Natal Solidário"
- **2 Doações**: Uma de roupas e uma em dinheiro

Para recriar o banco com dados de teste:
```bash
# Delete o banco existente e execute novamente
Remove-Item Data\app.db -ErrorAction SilentlyContinue
dotnet run
```

### Estrutura (resumo)
- **Usuario**: cd_cliente (PK), nome_completo, telefone, cpf (UNIQUE), cep, nome_usuario (UNIQUE), senha, email (UNIQUE)
- **Campanha**: cd_campanha (PK), nome_campanha, meta_arrecadacao, inicio, fim
- **Doacao**: cd_doacao (PK), cd_cliente (FK), cd_campanha (FK), nome_doacao, tipo_doacao, forma_arrecadacao, status_arrecadacao
- **Noticias**: cd_noticias (PK), cd_campanha (FK), titulo_noticia, data_noticia, autor, conteudo
- **Relatorio**: cd_relatorio (PK), cd_campanha (FK), tipo_relatorio, valor_gasto, data_relatorio

## ☁️ Deploy no Azure App Service (opcional)

> 📖 **Guia Completo**: Para um passo a passo detalhado, consulte o arquivo [GUIA_DEPLOY_AZURE.md](./GUIA_DEPLOY_AZURE.md)

### Pré-requisitos
- Conta Azure (Free tier disponível)
- Repositório GitHub com o código
- GitHub Actions configurado

### Resumo dos Passos (Método Recomendado - Mais Fácil) ⭐

1. **Criar App Service no Azure Portal**:
   - Acesse [Azure Portal](https://portal.azure.com)
   - Create a resource > Web App
   - Escolha:
     - **Runtime stack**: .NET 8 (LTS)
     - **Operating System**: Windows
     - **Pricing Plan**: **Free (F1)** ⭐ **GRATUITO - PERFEITO PARA PROJETO ACADÊMICO**
       - ✅ Totalmente gratuito
       - ⚠️ Pode ficar "dormindo" após 60 dias de inatividade (mas pode ser reativado)
   - Clique em "Review + Create"

2. **Configurar Deploy via GitHub (MÉTODO FÁCIL)** ⭐:
   - No App Service, vá em **Deployment Center**
   - Escolha **GitHub** como source
   - Faça login e autorize o Azure
   - Selecione seu repositório e branch (`main`)
   - Clique em **Save**
   - ✅ **Pronto!** O Azure cria tudo automaticamente (workflow, secrets, etc.)
   - O primeiro deploy pode levar 5-10 minutos

3. **Configurar Connection String**:
   - No App Service, vá em **Configuration** > **Application settings**
   - Clique em **New application setting**
   - **Name**: `ConnectionStrings:Default`
   - **Value**: `Data Source=D:\home\site\wwwroot\Data\app.db`
   - Clique em **Save**

4. **Testar**:
   - Acesse: `https://seuapp.azurewebsites.net/api/health`
   - Deve retornar: `{ "status": "ok" }`
   - Swagger: `https://seuapp.azurewebsites.net/swagger`
   - Listar usuários: `https://seuapp.azurewebsites.net/api/usuarios`

> 💡 **Dica**: Se preferir mais controle, consulte o [GUIA_DEPLOY_AZURE.md](./GUIA_DEPLOY_AZURE.md) para ver o método manual com publish profile (mais complexo).

### ⚠️ Importante

- **Método Automático**: Com o método do Deployment Center, o Azure cria o workflow automaticamente - você não precisa editar nada!
- **Swagger**: Está habilitado em produção para facilitar testes
- **Banco de Dados**: Será criado automaticamente na primeira execução no Azure
- **Primeiro Deploy**: Pode levar 5-10 minutos, seja paciente! 😊

### 💰 Plano Gratuito (Free F1)

**Vantagens:**
- ✅ Totalmente gratuito
- ✅ Perfeito para projetos acadêmicos
- ✅ HTTPS incluso
- ✅ 1GB de armazenamento (suficiente para SQLite)

**Limitações:**
- ⚠️ Pode pausar após 60 dias de inatividade (mas pode reativar facilmente)
- ⚠️ Primeira requisição após inatividade pode ser lenta (30-60s)
- ⚠️ Recursos limitados (mas suficientes para APIs pequenas/médias)

**Dica**: Para evitar pausa, faça uma requisição ao `/api/health` pelo menos uma vez por semana

## 🔧 Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- Serilog (console)
- Swagger/OpenAPI

## 📁 Estrutura do projeto

```
Servidor_PI/
├── Controllers/          # Controllers da API
├── Data/
│   ├── Maps/            # Fluent API configurations
│   └── AppDbContext.cs  # DbContext
├── Enums/               # Enumeradores
├── Models/              # Entidades
├── Repositories/
│   ├── Interfaces/      # Interfaces dos repositories
│   └── *.cs             # Implementações
├── Properties/
├── appsettings.json     # Configurações produção
├── appsettings.Development.json
├── Program.cs           # Entry point
└── README.md
```

## 📝 Logs

- Logs aparecem no console enquanto a API roda
- Para ver mais detalhes, cheque a saída do terminal

## ✅ Status Codes

- `200 OK` - Sucesso
- `201 Created` - Recurso criado
- `400 BadRequest` - Dados inválidos
- `404 NotFound` - Recurso não encontrado
- `409 Conflict` - Conflito (ex: email já existe)
- `500 InternalServerError` - Erro interno

## 🧪 Como testar

### Testes Manuais via PowerShell/CMD

1. **Health Check**:
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/health" -UseBasicParsing
```

2. **Listar Usuários** (deve retornar 2 usuários de teste):
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/usuarios" -UseBasicParsing
```

3. **Listar Doações** (deve retornar 2 doações de teste):
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/doacoes" -UseBasicParsing
```

4. **Listar Campanhas** (deve retornar 2 campanhas de teste):
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/campanhas" -UseBasicParsing
```

5. **Testar View de Doações Detalhadas**:
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/views/doacoes-detalhadas" -UseBasicParsing
```

6. **Via Swagger**: Acesse `http://localhost:5000/swagger` e teste os endpoints

### Testes via cURL

```bash
# Health Check
curl http://localhost:5000/api/health

# Listar Usuários
curl http://localhost:5000/api/usuarios

# Criar Usuário (exemplo)
curl -X POST http://localhost:5000/api/usuarios \
  -H "Content-Type: application/json" \
  -d '{"nome_completo":"Teste","nome_usuario":"teste","senha":"123","email":"teste@teste.com"}'
```

## 🐛 Problemas comuns

### Erro: "Unable to open database file"
- Verifique se a pasta `Data` existe e tem permissões
- No Azure, certifique-se que a connection string está correta
- A pasta `Data` é criada automaticamente na primeira execução

### Erro: "Foreign key constraint failed"
- Verifique se os registros relacionados existem antes de criar dependências
- Exemplo: crie um `Usuario` e `Campanha` antes de criar uma `Doacao`

### Erro: "Cannot set default value" em enum
- **Resolvido**: Enums convertidos para string não devem usar `HasDefaultValue` no mapeamento
- O valor padrão é definido no modelo (ex: `StatusArrecadacao.Pendente`)

### Aplicação não inicia
- Veja mensagens no console (possíveis erros detalhados)
- Confirme se a porta 5000 não está em uso: `netstat -ano | findstr :5000`
- Rode `dotnet clean` e `dotnet build` para compilar novamente

### Banco de dados não criado
- A aplicação cria o banco automaticamente na primeira execução
- Se necessário, delete o arquivo `Data/app.db` e execute novamente
- O `schema.sql` será executado automaticamente se o banco não existir

## 📞 Suporte

Para dúvidas, use o Swagger em `/swagger` ou veja o console.

