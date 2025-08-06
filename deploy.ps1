Write-Host "🚀 Starting Vehicle Resale API Deployment" -ForegroundColor Green

# Build da imagem Docker
Write-Host "📦 Building Docker image..." -ForegroundColor Yellow
docker build -t vehicle-resale-api:latest .

# Deploy no Kubernetes
Write-Host "☸️ Deploying to Kubernetes..." -ForegroundColor Yellow
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/sqlserver-pvc.yaml
kubectl apply -f k8s/sqlserver-deployment.yaml
kubectl apply -f k8s/sqlserver-service.yaml

# Aguardar SQL Server estar pronto
Write-Host "⏳ Waiting for SQL Server to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=sqlserver -n vehicle-resale --timeout=120s

# Deploy da API
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/api-service.yaml
kubectl apply -f k8s/ingress.yaml

# Verificar status
Write-Host "✅ Deployment complete!" -ForegroundColor Green
kubectl get all -n vehicle-resale

Write-Host "📊 To access the API:" -ForegroundColor Cyan
Write-Host "kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 8080:80"