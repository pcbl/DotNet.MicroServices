FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ConsumerService/ConsumerService.csproj", "ConsumerService/"]
RUN dotnet restore "ConsumerService/ConsumerService.csproj"
COPY . .
WORKDIR "/src/ConsumerService"
RUN dotnet build "ConsumerService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ConsumerService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ConsumerService.dll"]