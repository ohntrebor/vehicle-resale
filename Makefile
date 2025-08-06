.PHONY: help build run test clean docker-build docker-run k8s-deploy k8s-delete

help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

build: ## Build the application
	dotnet build

run: ## Run the application locally
	dotnet run --project VehicleResale.API

test: ## Run tests
	dotnet test

clean: ## Clean build artifacts
	dotnet clean
	rm -rf */bin */obj

restore: ## Restore NuGet packages
	dotnet restore

migration-add: ## Add a new migration (usage: make migration-add NAME=MigrationName)
	dotnet ef migrations add $(NAME) -p VehicleResale.Infrastructure -s VehicleResale.API -o Data/Migrations

migration-update: ## Update database with migrations
	dotnet ef database update -p VehicleResale.Infrastructure -s VehicleResale.API

docker-build: ## Build Docker image
	docker build -t vehicle-resale-api:latest .

docker-run: ## Run with Docker Compose
	docker-compose up -d

docker-stop: ## Stop Docker Compose
	docker-compose down

docker-logs: ## Show Docker logs
	docker-compose logs -f

k8s-deploy: ## Deploy to Kubernetes
	kubectl apply -f k8s/

k8s-delete: ## Delete from Kubernetes
	kubectl delete -f k8s/

k8s-status: ## Check Kubernetes status
	kubectl get all -n vehicle-resale

k8s-port-forward: ## Port forward to access the API
	kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 8080:80