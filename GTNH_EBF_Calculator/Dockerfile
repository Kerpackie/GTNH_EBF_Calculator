FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GTNH_EBF_Calculator/GTNH_EBF_Calculator.csproj", "GTNH_EBF_Calculator/"]
RUN dotnet restore "GTNH_EBF_Calculator/GTNH_EBF_Calculator.csproj"
COPY . .
WORKDIR "/src/GTNH_EBF_Calculator"
RUN dotnet build "GTNH_EBF_Calculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GTNH_EBF_Calculator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GTNH_EBF_Calculator.dll"]
