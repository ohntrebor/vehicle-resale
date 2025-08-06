#!/bin/bash

echo "🚀 Starting Vehicle Resale API Deployment"

# Build da imagem Docker
echo "📦 Building Docker image..."
docker build -t vehicle-resale-api:latest .

# Tag para registry (ajustar conforme seu registry)
# docker tag vehicle-resale-api:latest your-registry/vehicle-resale-api:latest
# docker push your-registry/vehicle-resale-api:latest

# Deploy no Kubernetes
echo "☸️ Deploying to Kubernetes..."
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/sqlserver-pvc.yaml
kubectl apply -f k8s/sqlserver-deployment.yaml
kubectl apply -f k8s/sqlserver-service.yaml

# Aguardar SQL Server estar pronto
echo "⏳ Waiting for SQL Server to be ready..."
kubectl wait --for=condition=ready pod -l app=sqlserver -n vehicle-resale --timeout=120s

# Deploy da API
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/api-service.yaml
kubectl apply -f k8s/ingress.yaml

# Verificar status
echo "✅ Deployment complete!"
kubectl get all -n vehicle-resale

echo "📊 To access the API:"
echo "kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 8080:80"