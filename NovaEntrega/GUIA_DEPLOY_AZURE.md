# 🚀 Guia Completo de Deploy no Azure

Este guia vai te ajudar a fazer o deploy do backend para o Azure App Service passo a passo.

## 📋 Pré-requisitos

- ✅ Conta Azure (pode criar uma gratuita em [portal.azure.com](https://portal.azure.com))
- ✅ Repositório GitHub com o código
- ✅ .NET 8 SDK instalado localmente (para testes)

## 🎯 Objetivo

Colocar o backend rodando na nuvem Azure, acessível publicamente via URL, com:
- ✅ Sistema funcionando
- ✅ Banco de dados SQLite configurado
- ✅ Deploy automático via GitHub Actions

---

## 📝 Passo 1: Criar o App Service no  Azure

1. **Acesse o Azure Portal**
   - Vá para [https://portal.azure.com](https://portal.azure.com)
   - Faça login na sua conta

2. **Criar um novo recurso**
   - Clique em **"Create a resource"** ou **"Criar um recurso"**
   - Procure por **"Web App"** ou **"App Service"**
   - Clique em **"Create"** ou **"Criar"**

3. **Configurar o App Service**
   
   **Basics (Básico):**
   - **Subscription**: Escolha sua assinatura
   - **Resource Group**: Crie um novo ou use existente
   - **Name**: Escolha um nome único (ex: `servidor-pi-12345`)
     - ⚠️ **ANOTE ESTE NOME!** Você vai precisar dele no workflow
   - **Publish**: `Code`
   - **Runtime stack**: `.NET 8 (LTS)`
   - **Operating System**: `Windows`
   - **Region**: Escolha a região mais próxima (ex: `Brazil South`)

   **App Service Plan:**
   - **Plan**: Criar novo plano
   - **Sku and size**: **Free (F1)** ⭐ **RECOMENDADO PARA PROJETO ACADÊMICO**
     - ✅ Totalmente gratuito
     - ✅ Perfeito para testes e projetos escolares
     - ⚠️ Limitações: 1GB de armazenamento, sem SSL customizado, pode ficar "dormindo" após inatividade
     - ⚠️ Após 60 dias de inatividade, o App Service pode ser pausado (mas pode ser reativado)
     - 💡 **Dica**: Para evitar pausa, faça uma requisição pelo menos uma vez por semana

4. **Review + Create**
   - Revise as configurações
   - Clique em **"Create"** ou **"Criar"**
   - Aguarde a criação (pode levar alguns minutos)

---

## 🚀 Passo 2: Configurar Deploy do GitHub (MÉTODO RECOMENDADO - MAIS FÁCIL)

### 💡 Por que usar este método?
- ✅ Não precisa baixar publish profile
- ✅ Não precisa configurar secrets no GitHub manualmente
- ✅ O Azure cria tudo automaticamente
- ✅ Perfeito para plano gratuito
- ⚠️ Primeiro deploy pode demorar um pouco mais (5-10 minutos)

### Passos:

1. **Acessar Deployment Center**
   - No App Service criado, vá para **"Deployment Center"** ou **"Centro de Implantação"**
   - Está no menu lateral esquerdo

2. **Configurar GitHub**
   - Clique em **"Settings"** ou **"Configurações"** (se necessário)
   - Em **"Source"**, escolha **"GitHub"**
   - Clique em **"Authorize"** ou **"Autorizar"** para conectar sua conta GitHub
   - Faça login na sua conta GitHub
   - Autorize o Azure a acessar seus repositórios

3. **Selecionar Repositório**
   - **Organization**: Escolha sua conta (ou organização)
   - **Repository**: Selecione o repositório do projeto
   - **Branch**: Escolha `main` (ou `master` se for o caso)
   - **Runtime stack**: Deve estar como `.NET 8` automaticamente
   - **Build provider**: Deixe como **"GitHub Actions"** (recomendado)

4. **Salvar**
   - Clique em **"Save"** ou **"Salvar"**
   - O Azure vai criar automaticamente:
     - ✅ Workflow do GitHub Actions
     - ✅ Secret necessário no GitHub
     - ✅ Configuração de deploy

5. **Aguardar Primeiro Deploy**
   - O primeiro deploy pode levar 5-10 minutos
   - Você pode acompanhar em:
     - **Deployment Center** → aba **"Logs"**
     - Ou no GitHub → **Actions** → ver o workflow rodando

6. **Verificar Deploy**
   - Após o deploy, você verá uma mensagem de sucesso
   - A URL do seu app já estará funcionando!

### ✅ Pronto! Deploy configurado automaticamente!

**Observação**: O Azure criou automaticamente o workflow `.github/workflows/azure-webapps-deploy.yml` no seu repositório. Você pode verificar e até editar se necessário.

---

## 🔧 Passo 3: Configurar Connection String

1. **Acessar Configurações**
   - Após a criação, vá para o recurso criado
   - No menu lateral, vá em **"Configuration"** ou **"Configuração"**
   - Clique na aba **"Application settings"** ou **"Configurações do aplicativo"**

2. **Adicionar Connection String**
   - Clique em **"+ New application setting"** ou **"+ Nova configuração do aplicativo"**
   - **Name**: `ConnectionStrings:Default`
   - **Value**: `Data Source=D:\home\site\wwwroot\Data\app.db`
   - Clique em **"OK"**
   - Clique em **"Save"** no topo da página
   - Aguarde a aplicação reiniciar

---

## 📋 MÉTODO ALTERNATIVO: Deploy Manual com Publish Profile

> ⚠️ **Se você já fez o deploy pelo método do GitHub acima, pule esta seção!**
> 
> Este método é mais complexo e só é necessário se você quiser mais controle.

### Passo 3A: Baixar Publish Profile

1. **Obter o Profile**
   - No App Service, vá em **"Overview"** ou **"Visão geral"**
   - Clique em **"Get publish profile"** ou **"Obter perfil de publicação"**
   - O arquivo `.PublishSettings` será baixado
   - **IMPORTANTE**: Abra este arquivo e copie TODO o conteúdo

### Passo 3B: Configurar GitHub Secret

1. **Acessar Secrets do Repositório**
   - Vá para o seu repositório GitHub
   - Clique em **"Settings"** → **"Secrets and variables"** → **"Actions"**
   - Clique em **"New repository secret"**

2. **Adicionar o Secret**
   - **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - **Secret**: Cole TODO o conteúdo do arquivo `.PublishSettings`
   - Clique em **"Add secret"**

### Passo 3C: Editar o Workflow

1. **Editar o Workflow**
   - No seu repositório, vá para `.github/workflows/azure-appservice.yml`
   - Na linha que diz: `AZURE_WEBAPP_NAME: servidor-pi`
   - **Substitua pelo nome do seu App Service**

2. **Commit e Push**
   ```bash
   git add .github/workflows/azure-appservice.yml
   git commit -m "Configurar deploy Azure"
   git push origin main
   ```

---

## 🚀 Passo 4: Fazer o Deploy (se usou método alternativo)

1. **Verificar o Workflow** (só se usou método alternativo)
   - Após o push, vá em **"Actions"** no GitHub
   - Você verá o workflow rodando
   - Aguarde ele completar (pode levar 5-10 minutos)

2. **Verificar Deploy**
   - Se der tudo certo, você verá um ✅ verde
   - Se der erro, clique no workflow para ver os detalhes

---

## ✅ Passo 5: Testar a Aplicação

1. **Obter a URL**
   - No Azure Portal, vá para o App Service
   - Em **"Overview"**, copie a **URL** (ex: `https://seu-app.azurewebsites.net`)

2. **Testar Endpoints**
   
   **Health Check:**
   ```
   https://seu-app.azurewebsites.net/api/health
   ```
   Deve retornar: `{"status":"ok"}`

   **Swagger:**
   ```
   https://seu-app.azurewebsites.net/swagger
   ```
   Deve abrir a documentação da API

   **Listar Usuários:**
   ```
   https://seu-app.azurewebsites.net/api/usuarios
   ```
   Deve retornar os 2 usuários de teste

---

## 🔍 Troubleshooting

### Erro no Deploy

**Problema**: Workflow falha no deploy
- ✅ Verifique se o secret `AZURE_WEBAPP_PUBLISH_PROFILE` está configurado corretamente
- ✅ Verifique se o nome do App Service no workflow está correto
- ✅ Verifique os logs do workflow no GitHub Actions

### Erro 500 na API

**Problema**: API retorna erro 500
- ✅ Verifique se a connection string está configurada no Azure
- ✅ Verifique os logs do App Service em **"Log stream"** ou **"Application Insights"**
- ✅ Verifique se o banco foi criado (pode levar alguns segundos na primeira execução)

### Banco de Dados não Criado

**Problema**: Endpoints retornam dados vazios
- ✅ A aplicação cria o banco automaticamente na primeira requisição
- ✅ Aguarde alguns segundos após o primeiro deploy
- ✅ Faça uma requisição para `/api/health` primeiro
- ✅ Depois teste os outros endpoints

### Swagger não Abre

**Problema**: Swagger retorna 404
- ✅ Verifique se está em modo Development (Swagger só aparece em Development)
- ✅ No Azure, configure a variável de ambiente `ASPNETCORE_ENVIRONMENT` como `Development` se quiser Swagger
- ✅ Ou acesse diretamente os endpoints da API

---

## 📊 Checklist Final

Antes de entregar, verifique:

- [ ] App Service criado no Azure (plano Free F1)
- [ ] Deploy configurado via GitHub (método recomendado) OU manualmente
- [ ] Connection string configurada no Azure
- [ ] Primeiro deploy concluído com sucesso
- [ ] Health check funcionando: `https://seu-app.azurewebsites.net/api/health`
- [ ] Endpoints retornando dados: `https://seu-app.azurewebsites.net/api/usuarios`
- [ ] Swagger acessível: `https://seu-app.azurewebsites.net/swagger`

---

## 🎉 Pronto!

Se tudo estiver funcionando, seu backend está rodando na nuvem Azure! 🚀

**URL da sua API**: `https://seu-app.azurewebsites.net`

Você pode usar esta URL para:
- ✅ Fazer requisições da aplicação frontend
- ✅ Testar endpoints via Postman/Insomnia
- ✅ Compartilhar com colegas/avaliadores

---

## 💰 Informações Importantes sobre o Plano Gratuito (Free F1)

### ✅ O que você tem de graça:
- ✅ App Service totalmente funcional
- ✅ 1GB de armazenamento (mais que suficiente para SQLite)
- ✅ HTTPS habilitado automaticamente
- ✅ Suporte a .NET 8
- ✅ Sem limite de requisições (dentro do razoável)

### ⚠️ Limitações do Plano Gratuito:
1. **Pode "adormecer" após inatividade**
   - Se ninguém acessar por 60 dias, o App Service pode ser pausado
   - **Solução**: Faça uma requisição pelo menos uma vez por semana
   - Se pausar, basta acessar o portal e clicar em "Start" para reativar

2. **Primeira requisição pode ser lenta**
   - Após inatividade, a primeira requisição pode levar 30-60 segundos
   - Isso é normal! O Azure está "acordando" o serviço
   - Requisições subsequentes são rápidas

3. **Sem SSL customizado**
   - Você usa o domínio padrão: `seu-app.azurewebsites.net`
   - Já vem com HTTPS, mas não pode usar domínio próprio

4. **Recursos limitados**
   - 1GB de RAM
   - CPU compartilhada (mas suficiente para APIs pequenas/médias)

### 💡 Dicas para o Plano Gratuito:

**Evitar que o App Service "durma":**
- Faça um cron job ou script que acessa `/api/health` uma vez por semana
- Ou use serviços gratuitos como UptimeRobot para monitorar
- Ou simplesmente acesse manualmente de vez em quando

**Se o App Service pausar:**
1. Acesse o Azure Portal
2. Vá para seu App Service
3. Clique em **"Start"** ou **"Iniciar"**
4. Aguarde 1-2 minutos
5. Pronto! Está funcionando novamente

**Performance:**
- Para projetos acadêmicos e testes, o plano Free é perfeito
- Se precisar de mais recursos, pode fazer upgrade depois (mas não é necessário para este projeto)

---

## 📞 Suporte

Se encontrar problemas:
1. Verifique os logs do App Service no Azure Portal
2. Verifique os logs do GitHub Actions
3. Consulte o README.md para mais detalhes
4. Verifique se todas as configurações estão corretas

