.PHONY: help build run test clean restore migration-add migration-remove migration-update docker-build docker-run docker-stop docker-logs docker-clean k8s-start k8s-deploy k8s-delete k8s-status k8s-port-forward k8s-logs k8s-clean dev-setup

## 📋 Mostra esta ajuda com todos os comandos disponíveis
help:
	@echo "🚀 Vehicle Resale API - Comandos Disponíveis:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

# ========================================
# 🔧 COMANDOS DE DESENVOLVIMENTO LOCAL
# ========================================

## 📦 Restaura pacotes NuGet do projeto
restore: 
	@echo "📦 Restaurando pacotes NuGet..."
	dotnet restore

## 🔨 Compila a aplicação .NET
build: 
	@echo "🔨 Compilando aplicação..."
	dotnet build

## ▶️  Executa a aplicação localmente
run: 
	@echo "▶️ Iniciando aplicação local..."
	@echo "🌐 API disponível em: http://localhost:5000"
	dotnet run --project VehicleResale.API

## 🧪 Executa todos os testes do projeto
test: 
	@echo "🧪 Executando testes..."
	dotnet test --verbosity normal

## 🧹 Remove arquivos de build e temporários
clean:
	@echo "🧹 Limpando arquivos temporários..."
	dotnet clean
	rm -rf */bin */obj

## 🛠️  Configuração inicial completa para desenvolvimento
dev-setup: restore build 
	@echo "✅ Ambiente de desenvolvimento configurado!"

# ========================================
# 🗄️ COMANDOS DE BANCO DE DADOS
# ========================================

## ➕ Adiciona nova migration (uso: make migration-add NAME=NomeDaMigracao)
migration-add: 
	@echo "➕ Adicionando migration: $(NAME)"
	dotnet ef migrations add $(NAME) -p VehicleResale.Infrastructure -s VehicleResale.API -o Data/Migrations

## ➖ Remove última migration
migration-remove: 
	@echo "➖ Removendo última migration..."
	dotnet ef migrations remove -p VehicleResale.Infrastructure -s VehicleResale.API

## 🔄 Atualiza banco de dados com migrations pendentes
migration-update: 
	@echo "🔄 Atualizando banco de dados..."
	dotnet ef database update -p VehicleResale.Infrastructure -s VehicleResale.API

## 📊 Mostra status das migrations
migration-status: 
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
	@powershell -Command "minikube docker-env | Invoke-Expression; docker build -t vehicle-resale-api:latest ."
	@echo "✅ Imagem construída no Minikube!"

## 🚀 Faz deploy da aplicação no Kubernetes
k8s-deploy:
	@echo "🚀 Fazendo deploy no Kubernetes..."
	kubectl apply -f k8s/
	@echo "✅ Deploy realizado!"
	@echo "⏳ Aguarde os pods ficarem prontos..."
	kubectl wait --for=condition=ready pod -l app=vehicle-resale-api -n vehicle-resale --timeout=300s
	@echo "🔗 Para acessar a API, execute: make k8s-port-forward"
	@echo "🌐 Depois acesse: http://localhost:9000/swagger/index.html"

## 🗑️ Remove aplicação do Kubernetes
k8s-delete:
	@echo "🗑️ Removendo aplicação do Kubernetes..."
	kubectl delete -f k8s/

## 📊 Mostra status dos recursos no Kubernetes
k8s-status: 
	@echo "📊 Status dos recursos Kubernetes:"
	kubectl get all -l app=vehicle-resale-api -n vehicle-resale
	@echo ""
	@echo "📋 Pods detalhados:"
	kubectl get pods -o wide -n vehicle-resale

## 🌐 Configurando portal manualmente - port-forward para acessar API (http://localhost:9000/swagger/index.html)
k8s-port-forward: 
	@echo "🌐 Configurando acesso à API via port-forward..."
	@echo "🔗 API disponível em: http://localhost:9000/swagger/index.html"
	@echo "⏹️ Para parar: Ctrl+C"
	kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 9000:80

## 📋 Mostra logs da aplicação no Kubernetes
k8s-logs:
	@echo "📋 Logs da aplicação:"
	kubectl logs -l app=vehicle-resale-api -n vehicle-resale -f

## 🔧 Acessa shell do pod da aplicação
k8s-shell: 
	@echo "🔧 Acessando shell do pod..."
	kubectl exec -it $$(kubectl get pod -l app=vehicle-resale-api -n vehicle-resale -o jsonpath='{.items[0].metadata.name}') -- /bin/bash

## 🔄 Reinicia deployment no Kubernetes
k8s-restart: 
	@echo "🔄 Reiniciando deployment..."
	kubectl rollout restart deployment/vehicle-resale-api-deployment -n vehicle-resale
	kubectl rollout status deployment/vehicle-resale-api-deployment -n vehicle-resale

## 🧹 Para Minikube e limpa recursos
k8s-clean:
	@echo "🧹 Limpando ambiente Kubernetes..."
	kubectl delete all --all -n vehicle-resale
	kubectl delete namespace vehicle-resale
	minikube stop
	minikube delete

## 📊 Abre dashboard do Kubernetes
k8s-dashboard: 
	@echo "📊 Abrindo dashboard do Kubernetes..."
	minikube dashboard

## 🔄 NOVO: Deploy rápido após mudanças no código
k8s-redeploy:
	@echo "🔄 Fazendo redeploy após mudanças..."
	@echo "🔨 Reconstruindo imagem..."
	@powershell -Command "minikube docker-env | Invoke-Expression; docker build -t vehicle-resale-api:latest ."
	@echo "🔄 Reiniciando deployment..."
	kubectl rollout restart deployment/vehicle-resale-api-deployment -n vehicle-resale
	kubectl rollout status deployment/vehicle-resale-api-deployment -n vehicle-resale
	@echo "✅ Redeploy concluído!"
	@echo "🌐 API disponível via: make k8s-port-forward"

## 🔍 NOVO: Verificar recursos em todos os namespaces
k8s-check-all:
	@echo "🔍 Verificando recursos em todos os namespaces..."
	kubectl get all --all-namespaces | grep vehicle-resale || echo "❌ Nenhum recurso encontrado"
	@echo ""
	@echo "📋 Namespaces disponíveis:"
	kubectl get namespaces

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

## 🚀 Setup completo Minikube em um comando único
k8s-full-deploy:
	@echo "🚀 Iniciando setup completo do Minikube..."
	@echo "🎯 1/6 - Iniciando Minikube..."
	minikube start --driver=docker
	@echo "🔧 2/6 - Configurando Docker do Minikube..."
	@powershell -Command "minikube docker-env | Invoke-Expression; docker build -t vehicle-resale-api:latest ."
	@echo "📁 3/6 - Criando namespace..."
	kubectl apply -f k8s/namespace.yaml
	@echo "⚙️ 4/6 - Aplicando configurações..."
	kubectl apply -f k8s/configmap.yaml k8s/secret.yaml
	@echo "🚀 5/6 - Fazendo deploy da aplicação..."
	kubectl apply -f k8s/
	@echo "⏳ Aguardando pods ficarem prontos..."
	kubectl wait --for=condition=ready pod -l app=vehicle-resale-api -n vehicle-resale --timeout=300s
	@echo "🌐 6/6 - Configurando port-forward na porta 9000..."
	@echo ""
	@echo "✅ Setup Minikube completo finalizado!"
	kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 9000:80
	@echo "🔗 API disponível em: http://localhost:9000/swagger/index.html"
	@echo "⏹️ Para parar o port-forward: Ctrl+C"

## 🧹 Limpeza completa do Minikube e Kubernetes
k8s-full-clean:
	@echo "🧹 Iniciando limpeza completa do Minikube..."
	@echo "🗑️ 1/4 - Removendo aplicação do Kubernetes..."
	kubectl delete -f k8s/ || echo "⚠️ Alguns recursos já foram removidos"
	@echo "🗑️ 2/4 - Removendo namespace..."
	kubectl delete namespace vehicle-resale || echo "⚠️ Namespace já foi removido"
	@echo "🗑️ 3/4 - Parando Minikube..."
	minikube stop || echo "⚠️ Minikube já estava parado"
	@echo "🗑️ 4/4 - Removendo cluster Minikube..."
	minikube delete || echo "⚠️ Cluster já foi removido"
	@echo ""
	@echo "✅ Limpeza completa finalizada!"
	@echo "💡 Para recriar tudo: make k8s-full-deploy"


# ========================================
# 📚 COMANDOS DE INFORMAÇÃO
# ========================================

## ℹ️  Mostra informações do ambiente
info: 
	@echo "ℹ️ Informações do Ambiente:"
	@echo "📁 Projeto: Vehicle Resale API"
	@echo "🔧 .NET Version: $$(dotnet --version)"
	@echo "🐳 Docker Version: $$(docker --version)"
	@echo "☸️ Kubectl Version: $$(kubectl version --client --short 2>/dev/null || echo 'Kubectl não instalado')"
	@echo "🎯 Minikube Status: $$(minikube status 2>/dev/null || echo 'Minikube não iniciado')"