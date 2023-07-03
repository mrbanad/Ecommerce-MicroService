FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /Publish/Moon

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CategoryServices.csproj", "./"]
RUN dotnet restore "CategoryServices.csproj"
COPY . .
WORKDIR "./"
RUN dotnet build "CategoryServices.csproj" -c Release -o /Publish/Moon/build

FROM build AS publish
RUN dotnet publish "CategoryServices.csproj" -c Release -o /Publish/Moon/publish

FROM base AS final
WORKDIR /Publish/Moon
COPY --from=publish /Publish/Moon/publish ./
ENTRYPOINT ["dotnet", "CategoryServices.dll"]
