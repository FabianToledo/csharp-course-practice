#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["HelloK8sFromDotNet/HelloK8sFromDotNet.csproj", "HelloK8sFromDotNet/"]
RUN dotnet restore "HelloK8sFromDotNet/HelloK8sFromDotNet.csproj"
COPY . .
WORKDIR "/src/HelloK8sFromDotNet"
RUN dotnet build "HelloK8sFromDotNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelloK8sFromDotNet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelloK8sFromDotNet.dll"]