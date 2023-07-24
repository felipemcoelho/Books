FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["src/Books.API/Books.API.csproj", "./Books.API/"]
COPY ["src/Books.Core/Books.Core.csproj", "./Books.Core/"]
COPY ["src/Books.Infrastructure/Books.Infrastructure.csproj", "./Books.Infrastructure/"]
COPY ["tests/Books.UnitTests/Books.UnitTests.csproj", "./Books.UnitTests/"]

RUN dotnet restore "./Books.API/Books.API.csproj"
RUN dotnet restore "./Books.Core/Books.Core.csproj"
RUN dotnet restore "./Books.Infrastructure/Books.Infrastructure.csproj"
RUN dotnet restore "./Books.UnitTests/Books.UnitTests.csproj"

COPY src/ .

WORKDIR "/src/Books.API"
RUN dotnet build "Books.API.csproj" -c Release -o /app/build

WORKDIR "/src/Books.UnitTests"
RUN dotnet test --no-restore

FROM build AS publish
WORKDIR "/src/Books.API"
RUN dotnet publish "Books.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Books.API.dll"]
EXPOSE 80