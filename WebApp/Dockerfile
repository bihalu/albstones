FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG VERSION
ENV VERSION ${VERSION}
ARG ASSEMBLY_VERSION
ENV ASSEMBLY_VERSION ${ASSEMBLY_VERSION}
WORKDIR /src
COPY ["WebApp/WebApp.csproj", "WebApp/"]
RUN dotnet restore "./WebApp/WebApp.csproj"
COPY . .
WORKDIR "/src/WebApp"
RUN dotnet build "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build -p:Version=$VERSION -p:AssemblyVersion=$ASSEMBLY_VERSION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG VERSION
ENV VERSION ${VERSION}
ARG ASSEMBLY_VERSION
ENV ASSEMBLY_VERSION ${ASSEMBLY_VERSION}
RUN dotnet publish "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish -p:UseAppHost=false -p:Version=$VERSION -p:AssemblyVersion=$ASSEMBLY_VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "albstones.dll"]
