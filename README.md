# Health Checks Library [AspNetCore]
Health Checks in ASP.NET Core v3.1

## EndPoints created by the implementation of Health Checks:

##### localhost:port/health
##### localhost:port/health/readness
##### localhost:port/health/liveness
  
## Implementation
[Health Checks - Inspired by Microsoft Documentation](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1)

## Sponsor
[![Vasconcellos Solutions](https://vasconcellos.solutions/assets/open-source/images/company/vasconcellos-solutions-small-icon.jpg)](https://www.vasconcellos.solutions)
### Vasconcellos IT Solutions

## Context of use for the Health Checks Library
Health checks are essential mechanisms in distributed systems, such as containerized applications and microservices, to ensure the availability and resilience of system components. They allow services to monitor and verify the health of their components, identifying potential issues and proactively acting to resolve them.

Kubernetes, a widely used container orchestration platform, offers two main types of health checks: readiness and liveness probes. These features help ensure that applications deployed in Kubernetes are always up and running for users.

The readiness probe is used to determine whether a container is ready to receive traffic. When a container is initialized, it may need some time to set up its environment and load essential data before it starts accepting connections. The readiness probe ensures that the service only begins receiving traffic when it is fully prepared to process it. If a container fails the readiness check, Kubernetes will not include it in the service's load balancing.

On the other hand, the liveness probe is used to check if a container is functioning correctly. If a container encounters issues, such as crashes or failures, the liveness probe will detect the failure and signal Kubernetes to restart the container. This ensures that applications continue running even when they face internal problems.

Health checks, including readiness and liveness probes, are fundamental to keeping large-scale applications running efficiently and reliably. They help ensure service availability, quick recovery from failures, and the ability to dynamically scale based on application needs. Implementing these mechanisms in a Kubernetes environment can significantly improve the stability and reliability of applications, resulting in a better experience for end users.


