#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["1Cloud.S3.API/1Cloud.S3.API.csproj", "1Cloud.S3.API/"]
RUN dotnet restore "1Cloud.S3.API/1Cloud.S3.API.csproj"
COPY . .
WORKDIR "/src/1Cloud.S3.API"
RUN dotnet build "1Cloud.S3.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "1Cloud.S3.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "1Cloud.S3.API.dll"]