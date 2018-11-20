FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Api/ /Api/
RUN ls -la /Api/*
COPY Infrastructure/ /Infrastructure/
RUN ls -la /Infrastructure/*
RUN dotnet restore /Api/Jarvis.Api.Service/Jarvis.Api.Service.csproj
WORKDIR /Api/Jarvis.Api.Service
RUN dotnet build Jarvis.Api.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Jarvis.Api.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Jarvis.Api.Service.dll"]
