# Service registry

## Starting the service registry
This section describes how to start the service registry

## How to add services
This sections describes how to add services to the service registry

## Docker
This project supports docker runtime environment, for which you will need to download docker from here: https://www.docker.com/products/docker-desktop/.

For this project, be aware that express listens on a specfic port (can be found in /Endpoints), which must be the same port that is used in the docker file. 

Open a terminal and navigate to the root of the project.

### Development certificate

For this project, it is necessary to provide a development certificate to the docker container.

Open a terminal and navigate the root of the project. Run the commands below to create and trust the certificate

EXACT_PROJECT_NAME is default: ServiceRegistry
CREDENTIAL_PLACEHOLDER is a password you choose. Remember it, because it will be used to run the docker container.
```
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\<EXACT_PROJECT_NAME>.pfx -p <CREDENTIAL_PLACEHOLDER>
dotnet dev-certs https --trust
```

## Docker network

If you run other services like a repository or service registry through docker locally, you will also need to setup a network. This creates a connection between local docker containers which is essential for establishing a connection.

If you want to run the project(s) with docker compose, the network needs to be created before running the compose file.

To create a docker network run below the commands below in a terminal:

```
docker network create -d bridge data
```

## Docker Compose
It is recommended to use this approach to run the application. To run this project using docker-compose you need to follow the steps below:

Build the docker image:
```
docker-compose build
```
Run the docker image:
```
docker-compose up
```
Stop the docker image:
```
docker-compose down
```

## Dockerfile
Alternatively you can build the image directly from the Dockerfile by running the following commands from the root of the project:

Please beware, that only the last docker run will work - as this will provide the certificate to the docker container. 

```
docker build -t ServiceRegistry .
docker run -d -p 3001:3001 -p 3000:3000 --name ServiceRegistry dockerserviceregistry:latest
```

When running in docker, localhost and 127.0.0.1 will resolve to the container. If you want to access the outside host (e.g. your machine), you can add an entry to the container's /etc/hosts file. You can read more details on this here: https://www.howtogeek.com/devops/how-to-connect-to-localhost-within-a-docker-container/
This will make localhost available as your destination when requesting from your host-unit e.g. from postman or the browser, not between containers.

To access the outside host, write the following docker run command instead of the one written above:
```
docker run -d -p 3001:3001 -p 3000:3000 --add-host host.docker.internal:host-gateway --name ServiceRegistry dockerserviceregistry:latest
```

Here the value "host.docker.internal" maps to the container's host gateway, which matches the real localhost value. This name can be replaced with your own string.

To establish connections between containers, add a reference to the network by adding the below to your docker run:

```
--network=data
```

This run provides all the necessary information except for the certificate:

```
docker run -d -p 3001:3001 -p 3000:3000 --add-host host.docker.internal:host-gateway --network=data --name ServiceRegistry dockerserviceregistry:latest
```

The run command we recommend which includes the certificate:

```
docker run -d -p 3001:3001 -p 3000:3000 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=3000 -e ASPNETCORE_Kestrel__Certificates__Default__Password="<CREDENTIAL_PLACEHOLDER>" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/ServiceRegistry.pfx -v %USERPROFILE%\.aspnet\https:/https/ --name ServiceRegistry dockerserviceregistry:latest
```