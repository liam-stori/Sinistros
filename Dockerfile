FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SinistrosApi.Domain/SinistrosApi.Domain.csproj           SinistrosApi.Domain/
COPY SinistrosApi.Application/SinistrosApi.Application.csproj SinistrosApi.Application/
COPY SinistrosApi.Infrastructure/SinistrosApi.Infrastructure.csproj SinistrosApi.Infrastructure/
COPY SinistrosApi.Api/SinistrosApi.Api.csproj                 SinistrosApi.Api/

RUN dotnet restore SinistrosApi.Api/SinistrosApi.Api.csproj

COPY . .

RUN dotnet publish SinistrosApi.Api/SinistrosApi.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:7070
EXPOSE 7070
ENTRYPOINT ["dotnet", "SinistrosApi.Api.dll"]
