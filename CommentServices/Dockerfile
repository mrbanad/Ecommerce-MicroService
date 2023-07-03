FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /Publish/Moon

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CommentServices.csproj", "./"]
RUN dotnet restore "CommentServices.csproj"
COPY . .
WORKDIR "./"
RUN dotnet build "CommentServices.csproj" -c Release -o /Publish/Moon/build

FROM build AS publish
RUN dotnet publish "CommentServices.csproj" -c Release -o /Publish/Moon/publish

FROM base AS final
WORKDIR /Publish/Moon
COPY --from=publish /Publish/Moon/publish ./
ENTRYPOINT ["dotnet", "CommentServices.dll"]
