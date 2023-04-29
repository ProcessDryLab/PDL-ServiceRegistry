# ServiceRegistry
Services registry for PDL

## Docker
This project supports docker runtime environment.

To build the docker image run, download docker from https://www.docker.com/products/docker-desktop/. 
Run the following commands from the project root to build an image and run it. 

```
docker build -t dockerserviceregistry .
docker run -d -p 3000:3000 --name serviceregistry dockerserviceregistry
```