# ===========================================
# DOCKERFILE - Vehicle Resale API
# ===========================================
# Este arquivo define como construir a imagem Docker da aplicação
# Usa multi-stage build para otimizar o tamanho final da imagem

# STAGE 1: Build - Compila o código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos .csproj primeiro (para cache de layers do Docker)
# Isso permite que o Docker reutilize layers se as dependências não mudaram
COPY ["VehicleResale.API/VehicleResale.API.csproj", "VehicleResale.API/"]
COPY ["VehicleResale.Application/VehicleResale.Application.csproj", "VehicleResale.Application/"]
COPY ["VehicleResale.Domain/VehicleResale.Domain.csproj", "VehicleResale.Domain/"]
COPY ["VehicleResale.Infrastructure/VehicleResale.Infrastructure.csproj", "VehicleResale.Infrastructure/"]

# Restaura as dependências NuGet
RUN dotnet restore "VehicleResale.API/VehicleResale.API.csproj"

# Copia todo o código fonte
COPY . .

# Navega para o diretório da API
WORKDIR "/src/VehicleResale.API"

# Compila a aplicação em modo Release
RUN dotnet build "VehicleResale.API.csproj" -c Release -o /app/build

# STAGE 2: Publish - Prepara os arquivos para produção
FROM build AS publish
RUN dotnet publish "VehicleResale.API.csproj" -c Release -o /app/publish

# STAGE 3: Runtime - Imagem final otimizada apenas com runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expõe as portas que a aplicação usa
EXPOSE 80    # HTTP
EXPOSE 443   # HTTPS (se configurado)

# Copia os arquivos publicados do stage anterior
COPY --from=publish /app/publish .

# Define o comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "VehicleResale.API.dll"]

# Healthcheck opcional (descomente se quiser)
# HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
#   CMD curl -f http://localhost/api/health || exit 1