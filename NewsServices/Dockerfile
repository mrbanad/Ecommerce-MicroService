FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /Publish/Moon

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NewsServices.csproj", "./"]
RUN dotnet restore "NewsServices.csproj"
COPY . .
WORKDIR "./"
RUN dotnet build "NewsServices.csproj" -c Release -o /Publish/Moon/build

FROM build AS publish
RUN dotnet publish "NewsServices.csproj" -c Release -o /Publish/Moon/publish

FROM base AS final
WORKDIR /Publish/Moon
COPY --from=publish /Publish/Moon/publish ./
ENTRYPOINT ["dotnet", "NewsServices.dll"]
