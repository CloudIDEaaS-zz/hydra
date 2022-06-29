FROM mcr.microsoft.com/dotnet/core/runtime:3.1-nanoserver-1903
WORKDIR /app

COPY ./DockerBuildTest .

EXPOSE 80
VOLUME C:/app/config
VOLUME C:/app/logs

ENTRYPOINT ["dotnet", "DockerBuildTest.dll"]