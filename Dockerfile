FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Presentation/BookStoreTesting/BookStoreTesting.csproj", "Presentation/BookStoreTesting/"]
COPY ["Core/BookStoreTesting.Domain/BookStoreTesting.Domain.csproj", "Core/BookStoreTesting.Domain/"]
COPY ["Core/BookStoreTesting.Application/BookStoreTesting.Application.csproj", "Core/BookStoreTesting.Application/"]
RUN dotnet restore "Presentation/BookStoreTesting/BookStoreTesting.csproj"

COPY . .
WORKDIR "/src/Presentation/BookStoreTesting"
RUN dotnet build "BookStoreTesting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStoreTesting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStoreTesting.dll"]