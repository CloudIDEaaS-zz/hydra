# Delete every Docker containers
# Must be run first because images are attached to containers
docker rm -f $(docker ps -a -q)

# Delete every Docker image
docker rmi -f $(docker images -q)

docker image prune -af

cls
docker image ls
docker ps

# D:\ProgramData\Docker\tmp
# D:\ProgramData\Docker\windowsfilter
# https://teamtreehouse.com/library/the-entrypoint-and-cmd-instructions
# https://nukedbit.dev/how-to-specify-custom-run-arguments-for-docker-tools-in-visual-studio-2019/

cd "D:\MC\CloudIDEaaS\root\DockerBuildTest"
dotnet build "DockerBuildTest.csproj" --configuration release
dotnet publish "DockerBuildTest.csproj" --configuration Release --output staging

cls
cd "D:\MC\CloudIDEaaS\root\DockerBuildTest\staging"
docker build -t dockerbuildtest -f D:\MC\CloudIDEaaS\root\DockerBuildTest\staging\Dockerfile D:\MC\CloudIDEaaS\root\DockerBuildTest\staging

cls
docker image ls

#docker run -d -p 8080:80 --mount source=z,target=/app dockerbuildtest --name dockerbuildtest
docker run -d -p 8080:80 --name dockerbuildtest dockerbuildtest

# debug

docker image inspect dockerbuildtest

cls
docker ps

docker exec -it hydradevopsautomationservice:dev ipconfig
docker run -it hydradevopsautomationservice:dev ipconfig

docker build -t hydradevops -f D:\MC\CloudIDEaaS\root\HydraDevOps\Dockerfile D:\MC\CloudIDEaaS\root\HydraDevOps
docker image inspect 72adddef27e6
docker container inspect 2fbed12848fd

docker exec e457ee6db74b dir


# https://github.com/wagoodman/dive
# D:\Users\Ken\Downloads\dive_0.9.2_windows_amd64
# dive 1514e2afd02f

wget http://192.168.0.61:65400/api/user/token

dive 0520c1718b17