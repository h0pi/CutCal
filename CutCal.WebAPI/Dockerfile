FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5194

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY CutCal.Model/CutCal.Model.csproj CutCal.Model/
COPY CutCal.Common.Services/CutCal.Common.Services.csproj CutCal.Common.Services/
COPY CutCal.Services/CutCal.Services.csproj CutCal.Services/
COPY CutCal.WebAPI/CutCal.WebAPI.csproj CutCal.WebAPI/
RUN dotnet restore CutCal.WebAPI/CutCal.WebAPI.csproj

COPY CutCal.Model/ CutCal.Model/
COPY CutCal.Common.Services/ CutCal.Common.Services/
COPY CutCal.Services/ CutCal.Services/
COPY CutCal.WebAPI/ CutCal.WebAPI/
RUN dotnet publish CutCal.WebAPI/CutCal.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CutCal.WebAPI.dll"]
