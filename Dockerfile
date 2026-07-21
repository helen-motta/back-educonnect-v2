FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY back-educonnect.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish back-educonnect.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/data
ENV ASPNETCORE_URLS=http://+:5055
ENV Database__Provider=Sqlite
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/educonnect.db"
EXPOSE 5055
VOLUME ["/app/data"]
ENTRYPOINT ["dotnet", "back-educonnect.dll"]
