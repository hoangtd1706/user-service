#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ecoba.UserService/Ecoba.UserService.csproj", "Ecoba.UserService/"]
RUN dotnet restore "Ecoba.UserService/Ecoba.UserService.csproj"
COPY . .
WORKDIR "/src/Ecoba.UserService"
RUN dotnet build "Ecoba.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecoba.UserService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecoba.UserService.dll"]
