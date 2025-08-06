.PHONY: help build run test clean restore migration-add migration-remove migration-update docker-build docker-run docker-stop docker-logs docker-clean k8s-start k8s-deploy k8s-delete k8s-status k8s-port-forward k8s-logs k8s-clean dev-setup

help: ## 📋 Mostra esta ajuda com todos os comandos disponíveis
	@echo "🚀 Vehicle Resale API - Comandos Disponíveis:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

# ========================================
# 🔧 COMANDOS DE DESENVOLVIMENTO LOCAL
# ========================================

restore: ## 📦 Restaura pacotes NuGet do projeto
	@echo "📦 Restaurando pacotes NuGet..."
	dotnet restore

build: ## 🔨 Compila a aplicação .NET
	@echo "🔨 Compilando aplicação..."
	dotnet build

run: ## ▶️  Executa a aplicação localmente
	@echo "▶️ Iniciando aplicação local..."
	@echo "🌐 API disponível em: http://localhost:5000"
	dotnet run --project VehicleResale.API

test: ## 🧪 Executa todos os testes do projeto
	@echo "🧪 Executando testes..."
	dotnet test --verbosity normal

clean: ## 🧹 Remove arquivos de build e temporários
	@echo "🧹 Limpando arquivos temporários..."
	dotnet clean
	rm -rf */bin */obj

dev-setup: restore build ## 🛠️  Configuração inicial completa para desenvolvimento
	@echo "✅ Ambiente de desenvolvimento configurado!"

# ========================================
# 🗄️ COMANDOS DE BANCO DE DADOS
# ========================================

migration-add: ## ➕ Adiciona nova migration (uso: make migration-add NAME=NomeDaMigracao)
	@echo "➕ Adicionando migration: $(NAME)"
	dotnet ef migrations add $(NAME) -p VehicleResale.Infrastructure -s VehicleResale.API -o Data/Migrations

migration-remove: ## ➖ Remove última migration
	@echo "➖ Removendo última migration..."
	dotnet ef migrations remove -p VehicleResale.Infrastructure -s VehicleResale.API

migration-update: ## 🔄 Atualiza banco de dados com migrations pendentes
	@echo "🔄 Atualizando banco de dados..."
	dotnet ef database update -p VehicleResale.Infrastructure -s VehicleResale.API

migration-status: ## 📊 Mostra status das migrations
	@echo "📊 Status das migrations:"
	dotnet ef migrations list -p VehicleResale.Infrastructure -s VehicleResale.API

# ========================================
# 🐳 COMANDOS DOCKER COMPOSE
# ========================================


## 🔨 Constrói imagem Docker da aplicação
docker-build: 
	@echo "🔨 Construindo imagem Docker..."
	docker build -t vehicle-resale-api:latest .

## 🚀 Inicia todos os serviços com Docker Compose
docker-run: 
	@echo "🚀 Iniciando serviços com Docker Compose..."
	docker-compose up -d
	@echo "🌐 API disponível em: http://localhost:5000"
	@echo "🗄️ Banco de dados disponível na porta: 1433"

 ## ⏹️  Para todos os serviços do Docker Compose
docker-stop:
	@echo "⏹️ Parando serviços do Docker Compose..."
	docker-compose down

## 📋 Mostra logs dos containers em tempo real
docker-logs: 
	@echo "📋 Logs dos containers:"
	docker-compose logs -f

## 🔄 Reinicia todos os serviços Docker
docker-restart: docker-stop docker-run

## 🧹 Remove containers, volumes e imagens não utilizados
docker-clean: 
	@echo "🧹 Limpando recursos Docker..."
	docker-compose down -v --remove-orphans
	docker system prune -f

 ## 🗄️ Inicia apenas o banco de dados
docker-db-only:
	@echo "🗄️ Iniciando apenas SQL Server..."
	docker-compose up -d sqlserver

# ========================================
# ☸️ COMANDOS KUBERNETES
# ========================================

## 🎯 Inicia Minikube e configura ambiente Kubernetes
k8s-start: 
	@echo "🎯 Iniciando Minikube..."
	minikube start --driver=docker
	@echo "✅ Minikube iniciado!"
	kubectl get nodes

## 🔨 Constrói imagem no ambiente Minikube
k8s-build: 
	@echo "🔨 Configurando Docker do Minikube e construindo imagem..."
	eval $$(minikube docker-env) && docker build -t vehicle-resale-api:latest .
	@echo "✅ Imagem construída no Minikube!"

 ## 🚀 Faz deploy da aplicação no Kubernetes
k8s-deploy:
	@echo "🚀 Fazendo deploy no Kubernetes..."
	kubectl apply -f k8s/
	@echo "✅ Deploy realizado!"
	@echo "⏳ Aguarde os pods ficarem prontos..."
	kubectl wait --for=condition=ready pod -l app=vehicle-resale-api --timeout=300s

 ## 🗑️  Remove aplicação do Kubernetes
k8s-delete:
	@echo "🗑️ Removendo aplicação do Kubernetes..."
	kubectl delete -f k8s/

## 📊 Mostra status dos recursos no Kubernetes
k8s-status: 
	@echo "📊 Status dos recursos Kubernetes:"
	kubectl get all -l app=vehicle-resale-api
	@echo ""
	@echo "📋 Pods detalhados:"
	kubectl get pods -o wide

## 🌐 Configura port-forward para acessar API (http://localhost:8080)
k8s-port-forward: 
	@echo "🌐 Configurando acesso à API via port-forward..."
	@echo "🔗 API disponível em: http://localhost:8080"
	@echo "⏹️  Para parar: Ctrl+C"
	kubectl port-forward service/vehicle-resale-api-service 8080:80

## 📋 Mostra logs da aplicação no Kubernetes
k8s-logs:
	@echo "📋 Logs da aplicação:"
	kubectl logs -l app=vehicle-resale-api -f

## 🔧 Acessa shell do pod da aplicação
k8s-shell: 
	@echo "🔧 Acessando shell do pod..."
	kubectl exec -it $$(kubectl get pod -l app=vehicle-resale-api -o jsonpath='{.items[0].metadata.name}') -- /bin/bash

## 🔄 Reinicia deployment no Kubernetes
k8s-restart: 
	@echo "🔄 Reiniciando deployment..."
	kubectl rollout restart deployment/vehicle-resale-api-deployment

## 🧹 Para Minikube e limpa recursos
k8s-clean:
	@echo "🧹 Limpando ambiente Kubernetes..."
	kubectl delete all --all
	minikube stop
	minikube delete

## 📊 Abre dashboard do Kubernetes
k8s-dashboard: 
	@echo "📊 Abrindo dashboard do Kubernetes..."
	minikube dashboard

# ========================================
# 🔄 WORKFLOWS COMPLETOS
# ========================================

## 🐳 Setup completo com Docker (limpa + constrói + executa)
full-docker-setup: docker-clean docker-build docker-run 
	@echo "✅ Setup Docker completo finalizado!"

## ☸️ Setup completo com Kubernetes (inicia + constrói + deploy)
full-k8s-setup: k8s-start k8s-build k8s-deploy 
	@echo "✅ Setup Kubernetes completo finalizado!"
	@echo "🌐 Execute 'make k8s-port-forward' para acessar a API"

# ========================================
# 📚 COMANDOS DE INFORMAÇÃO
# ========================================

info: ## ℹ️  Mostra informações do ambiente
	@echo "ℹ️ Informações do Ambiente:"
	@echo "📁 Projeto: Vehicle Resale API"
	@echo "🔧 .NET Version: $$(dotnet --version)"
	@echo "🐳 Docker Version: $$(docker --version)"
	@echo "☸️ Kubectl Version: $$(kubectl version --client --short 2>/dev/null || echo 'Kubectl não instalado')"
	@echo "🎯 Minikube Status: $$(minikube status 2>/dev/null || echo 'Minikube não iniciado')"