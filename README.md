# Build & run
1.  Install docker & docker-compose: https://www.docker.com/
2.  Install node js https://nodejs.org/en/: 
3.  Download the latest dotnet 3.0 preview: https://dotnet.microsoft.com/download/dotnet-core/3.0
4.  Run all.bat (linux variant coming soon)
5.  Navigate to http://localhost:8081

# Lessons learned
- Event Sourcing might be hard to wrap your head around at first, but it also removes a lot of challenges that are posed by the impedance mismatch and distributed transactions. To me, it seems to be a very viable strategy, especially for microservices.


# Introduction 
I created this project to learn new technologies, especially event sourcing and microservices. The business case is a complex one (constructing yield curves), but not the main focus. Currently, the way of constructing curves is not correct, though I might change this in the future.

Inspiration taken from:
https://www.youtube.com/watch?v=6Fi5dRVxOvc - An Opinionated Approach to ASP.NET Core - Scott Allen
https://github.com/jbogard/ContosoUniversityDotNetCore-Pages
https://github.com/jbogard/ContosoUniversityDotNetCore
https://github.com/ardalis/CleanArchitecture
https://github.com/JasonGT/NorthwindTraders
https://github.com/matthewrenze/clean-architecture-demo
https://github.com/ivanpaulovich/clean-architecture-manga


# Demonstrated patterns / technologies
1.	Clean Architecture
	https://gist.github.com/ygrenzinger/14812a56b9221c9feca0b3621518635b
	http://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
	https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164
	https://www.pluralsight.com/courses/clean-architecture-patterns-practices-principles

2.	Domain Driven Design (use of Aggregate Roots, Value Objects, behaviour-rich domain model)
	https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215
	https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577
	https://lostechies.com/jimmybogard/2010/02/04/strengthening-your-domain-a-primer/
	https://www.pluralsight.com/courses/domain-driven-design-fundamentals
	https://www.pluralsight.com/courses/domain-driven-design-in-practice
	https://vimeo.com/43598193

3.	Command/Query Responsiblity Segregation
	https://www.pluralsight.com/courses/modern-software-architecture-domain-models-cqrs-event-sourcing
	https://www.pluralsight.com/courses/cqrs-in-practice
	https://vimeo.com/131633177

4.  Event Sourcing
	https://martinfowler.com/eaaDev/EventSourcing.html
	https://github.com/EdwinVW/pitstop
	https://github.com/rbanks54/microcafe

5.  Micro Services
    https://www.oreilly.com/library/view/building-microservices/9781491950340/

6.  Micro Frontends / Composite UI using distributed Vue Components
    https://markus.oberlehner.net/blog/distributed-vue-applications-loading-components-via-http/
    https://markus.oberlehner.net/blog/distributed-vue-applications-pushing-content-and-component-updates-to-the-client/

7.  Actor model with Akka.NET
    https://getakka.net/


# To do:
- Proper authentication/authorization
- Extensive unit tests
- Migrate at least a few microservices to F# or another functional language: suits the event sourced code, as well as the business case
- Use WebSockets for live updates
- Use Protobuff for serialization
