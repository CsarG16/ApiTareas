# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código fuente y publicar
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar la aplicación publicada
COPY --from=build /app/publish .

# Render usa la variable de entorno PORT
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ApiTareas.dll"]
