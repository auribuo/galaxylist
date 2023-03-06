FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Galaxylist/Galaxylist.csproj", "Galaxylist/"]
RUN dotnet restore "Galaxylist/Galaxylist.csproj"
COPY . .
WORKDIR "/src/Galaxylist"
RUN dotnet build "Galaxylist.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Galaxylist.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Galaxylist.dll"]
