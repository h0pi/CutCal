FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY CutCal.Model/CutCal.Model.csproj CutCal.Model/
COPY CutCal.Common.Services/CutCal.Common.Services.csproj CutCal.Common.Services/
COPY CutCal.Messaging/CutCal.Messaging.csproj CutCal.Messaging/
COPY CutCal.Recommender/CutCal.Recommender.csproj CutCal.Recommender/
COPY CutCal.Services/CutCal.Services.csproj CutCal.Services/
COPY CutCal.Worker/CutCal.Worker.csproj CutCal.Worker/
RUN dotnet restore CutCal.Worker/CutCal.Worker.csproj

COPY CutCal.Model/ CutCal.Model/
COPY CutCal.Common.Services/ CutCal.Common.Services/
COPY CutCal.Messaging/ CutCal.Messaging/
COPY CutCal.Recommender/ CutCal.Recommender/
COPY CutCal.Services/ CutCal.Services/
COPY CutCal.Worker/ CutCal.Worker/
RUN dotnet publish CutCal.Worker/CutCal.Worker.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CutCal.Worker.dll"]
