# .NET Microservices - Full Course [Implemented]

## Services

### PlatformService
- APIs
  - GetPlatforms
  - GetPlatformById
  - CreatePlatform

### CommandService
- APIs
  - GetCommandsByPlatform
  - GetCommandByPlatform
  - CreateCommandByPlatform

  - TestInboundConnection
  - GetPlatforms

### Rabbitmq
- [PlatformService] created & PublishNewPlatform
- [CommandService] ProcessEvent

### Grpc
- [PlatformService] [Grpc.Server] GetAllPlatforms 
- [CommandService] [Grpc.Client] init to call PlatformDataClient 

### Docker
- build image
    
      $ docker build -t <name:tag> .
      
- push to Docker hub
  
      $ docker push <name:tag>

### Kubernetes

- Deployments
  - commands-depl
  - platforms-depl
  - mssql-sql
  - rabbitmq-sql
- Services
  - ClusterIP
    - platforms-clusterip-srv
    - commands-clusterip-srv
    - mssql-clusterip-srv
  - LoadBalancer
    - platforms-lb-srv
    - mssql-lb-srv
    - rabbitmq-lb-srv
- Ingress
  - ingress-srv
- PersistentVolumeClaim
  - local-pvc

- Secret
```
(SA) password: At least 8 characters including uppercase, lowercase letters, base-10 digits and/or non-alphanumeric symbols.
  
$ k create secret generic mssql --from-literal=SA_PASSWORD="Strong@1234"
```

- minikube config
  - Dashboard
        
        $ minikube addons enable dashboard
        $ minikube dashboard
  - Ingress
        
        $ minikube addons enable ingress 
  - Service (Export LoadBalance endpoint)
        
        $ minikube tunnel

## References

- [.NET Microservices – Full Course](https://www.youtube.com/watch?v=DgVjEo3OGBI&list=LL&index=1&t=14s)
- [Github codes](https://github.com/binarythistle/S04E03---.NET-Microservices-Course-)
- [Docker Hub .Net Core SDK](https://hub.docker.com/_/microsoft-dotnet-sdk)
- [Docker Hub .Net Core Runtime](https://hub.docker.com/_/microsoft-dotnet-aspnet)
- [Minikube tunnel](https://minikube.sigs.k8s.io/docs/commands/tunnel/)