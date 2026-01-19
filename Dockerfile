FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Askly.Api/Askly.Api.csproj", "Askly.Api/"]
COPY ["Askly.Application/Askly.Application.csproj", "Askly.Application/"]
COPY ["Askly.Domain/Askly.Domain.csproj", "Askly.Domain/"]
COPY ["Askly.Infrastructure/Askly.Infrastructure.csproj", "Askly.Infrastructure/"]
RUN dotnet restore "Askly.Api/Askly.Api.csproj"
COPY . .
WORKDIR "/src/Askly.Api"
RUN dotnet build "./Askly.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Askly.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Askly.Api.dll"]
