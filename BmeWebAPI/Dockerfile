#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BmeWebAPI/BmeWebAPI.csproj", "BmeWebAPI/"]
COPY ["BmeModels/BmeModels.csproj", "BmeModels/"]
RUN dotnet restore "BmeWebAPI/BmeWebAPI.csproj"
COPY . .
WORKDIR "/src/BmeWebAPI"
RUN dotnet build "BmeWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BmeWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BmeWebAPI.dll"]