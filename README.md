# Lessons learned
- Clean architecture can be used for simple applications too, especially if you create a good template / base classes.
- For complex projects however, DISREGARD THE DONT REPEAT YOURSELF PRINCIPLE. Be REALLY carefull when introducing coupling between features, especially when using microservices.
- If you want to use bounded contexts, go for microservices (and ideally CQRS, eventSourcing, eventStore, eventBus, eventual consistency). Having many bounded contexts within the same dependency injection context is way too much hassle and has hardly any benefits.

# Introduction 
This project was created to showcase new patterns and technologies for the Osiris team, and as an example for a future solution architecture. 
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

	--> Interesting read on event sourcing: https://www.confluent.io/blog/event-sourcing-cqrs-stream-processing-apache-kafka-whats-connection/

4.	Specification Pattern (where-clauses as business logic, very unit-testable and re-usable)
	https://www.pluralsight.com/courses/csharp-specification-pattern
	https://enterprisecraftsmanship.com/2016/02/08/specification-pattern-c-implementation/

5.	Query Objects (possibility to specify paging and sorting from outside the persistence layer)
	--> Mostly based on readups on the Specification Pattern, but like the idea of seperating paging/sorting from the specification (Seperation of Concerns)

6.  Automapper projection for efficient querying
	http://docs.automapper.org/en/stable/Queryable-Extensions.html

7.	Reference Data as code (using SmartEnum)
	https://enterprisecraftsmanship.com/2016/03/17/reference-data-as-code/
	https://github.com/ardalis/SmartEnum
	https://docs.microsoft.com/nl-nl/ef/core/modeling/value-conversions

8.  Entity Framework Core Code First, using a rich domain model
	https://technet.microsoft.com/en-us/mt842503.aspx
	https://msdn.microsoft.com/en-us/magazine/mt826347.aspx
	https://www.youtube.com/watch?v=9Vp2iXlhK-s
	https://www.pluralsight.com/courses/playbook-ef-core-2-1-whats-new

9.  Fluent Validation
	https://fluentvalidation.net/

10.	Exception Filters in MVC

11.  Dependency Injection (using AutoFac)

# Not demonstrated but interesting reads:
- Event Sourcing
	https://martinfowler.com/eaaDev/EventSourcing.html
	https://www.confluent.io/blog/event-sourcing-cqrs-stream-processing-apache-kafka-whats-connection/

#Cross-cutting concerns:
- SmartEnum
- AutoMapper
- FluentValidation

# To do:
- Extensive unit tests for the potential NuGet packages