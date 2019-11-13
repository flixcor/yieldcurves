
# Build & run
 1.  Install [docker & docker-compose](https://www.docker.com/):
 2.  Install [node js](https://nodejs.org/en/)  
 3.  Download [dotnet 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)
 4.  Run all.bat (linux variant coming soon)
 6.  Navigate to http://localhost:8081

# Introduction 
I created this project to learn new technologies, especially event sourcing and microservices. The business case is a complex one: constructing [yield curves](https://www.investopedia.com/terms/y/yieldcurve.asp). I chose this business case precisely and only because it is complex: I wanted to try new technologies in a setting that goes beyond the usual examples one can find online. I have no real ambition to get the actual calculation of curves up to par with industry standards. This project is not at all ready for production.

# Demonstrated patterns / technologies
1.	Clean Architecture
	https://gist.github.com/ygrenzinger/14812a56b9221c9feca0b3621518635b
	http://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
	https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164
	https://www.pluralsight.com/courses/clean-architecture-patterns-practices-principles

2.	Domain Driven Design (use of Aggregate Roots, Value Objects, rich domain model)
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
    https://martinfowler.com/articles/micro-frontends.html
    https://markus.oberlehner.net/blog/distributed-vue-applications-loading-components-via-http/

8.  Actor model with Akka.NET
    https://getakka.net/
    
Inspiration taken from:
[An Opinionated Approach to ASP.NET Core - Scott Allen](https://www.youtube.com/watch?v=6Fi5dRVxOvc)
https://github.com/jbogard/ContosoUniversityDotNetCore

# Application
> A yield curve is a line that plots yields (interest rates) of bonds
> having equal credit quality but differing maturity. 
> The slope of the yield curve gives an idea of future interest 
> rate changes and economic activity.
> https://www.investopedia.com/terms/y/yieldcurve.asp

To construct a yield curve, you need data from the stock market (e.g. instrument prices), as wel as certain models that can be applied to this data. This application offers the following functionality:

 - Add _instrument definitions_ to the system (manually)
 - Assemble these instruments into _market curves_ (manually)
 - Add _pricing information_ for these instruments (manually)
 - Define _recipes_ in which the market curves should be manipulated into _calculated curves_ (manually)
 - Perform the actual _calculations_ when all required information is available (automatically)

# To do:
- Proper authentication/authorization
- Extensive unit tests
- Migrate at least a few microservices to F# or another functional language: suits the event sourced code, as well as the business case
