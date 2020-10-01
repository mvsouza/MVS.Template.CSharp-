FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE ${PORT:-80}
ENV PORT ${PORT:-80}
ENV ASPNETCORE_URLS http://+:${PORT:-80}

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY src/MVS.Template.CSharp.Domain/MVS.Template.CSharp.Domain.csproj src/MVS.Template.CSharp.Domain/MVS.Template.CSharp.Domain.csproj
COPY src/MVS.Template.CSharp.Application/MVS.Template.CSharp.Application.csproj src/MVS.Template.CSharp.Application/MVS.Template.CSharp.Application.csproj
COPY src/MVS.Template.CSharp.Infrastructure/MVS.Template.CSharp.Infrastructure.csproj src/MVS.Template.CSharp.Infrastructure/MVS.Template.CSharp.Infrastructure.csproj
COPY test/MVS.Template.CSharp.FunctionalTest/MVS.Template.CSharp.FunctionalTest.csproj test/MVS.Template.CSharp.FunctionalTest/MVS.Template.CSharp.FunctionalTest.csproj
COPY test/MVS.Template.CSharp.UnitTest/MVS.Template.CSharp.UnitTest.csproj test/MVS.Template.CSharp.UnitTest/MVS.Template.CSharp.UnitTest.csproj
COPY MVS.Template.CSharp.sln MVS.Template.CSharp.sln
RUN dotnet restore

COPY . .
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS testrunner
RUN dotnet test test/MVS.Template.CSharp.UnitTest/MVS.Template.CSharp.UnitTest.csproj
RUN dotnet test test/MVS.Template.CSharp.FunctionalTest/MVS.Template.CSharp.FunctionalTest.csproj
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet MVS.Template.CSharp.Infrastructure.dll