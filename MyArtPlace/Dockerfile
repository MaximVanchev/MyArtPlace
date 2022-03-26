#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MyArtPlace/MyArtPlace.csproj", "MyArtPlace/"]
RUN dotnet restore "MyArtPlace/MyArtPlace.csproj"
COPY . .
WORKDIR "/src/MyArtPlace"
RUN dotnet build "MyArtPlace.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyArtPlace.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyArtPlace.dll"]