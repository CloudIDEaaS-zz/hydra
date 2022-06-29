FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY . .
EXPOSE 80
VOLUME C:/app/config
VOLUME C:/app/logs
ENTRYPOINT ["dotnet", "DockerBuildTest.dll"]