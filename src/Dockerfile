# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia csproj e restaura dependências
COPY OpenCnpj.sln .
COPY OpenCnpj.WebApi/*.csproj ./OpenCnpj.WebApi/
RUN dotnet restore ./OpenCnpj.WebApi

# Copia tudo e publica
COPY . .
WORKDIR /src/OpenCnpj.WebApi
RUN dotnet publish -c Release -o /app

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "OpenCnpj.WebApi.dll"]
