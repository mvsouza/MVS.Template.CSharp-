# Package design overview   

Let's cover the package design decision made for this project and understand it's hidden technical background. 

This project was made using Domain-driven design (DDD) layering pattern. It has been slightly changed compared to the version introduced by Vaughn Vernon on _Implementing Domain Driven Design_.  

He changed the main approach defined previously by Eric Evans. He achieved a better design with Dependence Inversion Principle (DIP) and a clear Single Responsibility Principle (SRP) definition.

Following his suggestion, the position of the infrastructure layer was inverted, putting it on top despite the standard model, that leave it in the bottom. So what was changed here from Vernon's model? 

The UI and Infrastructure layers have been squashed into a single package, although their responsibilities are still separate.  The main reason is that *aspnet core*, a UI reference, is tightly coupled to programs `Startup`, an Infrastructure class. Anyway, the API logic is too thin to be on its own, so it's controllers are located on an API folder inside Infrastructure.

It's not usual to have an application layer in *dotnet* projects, although this layer should be decoupled from the layer UI more often. The UI changes should not impact the application layer. For example, if our SOA applications protocol of communication changes from gRPC to HTPP Rest, it's expected to change only the UI layer. I could even have both implementations in separate solutions.    

Application's logic is exposed to the UI using events. This project leverages of event-driven design, which is enabled by Mediatr NuGet package. So the UI layer views depend only on the classes of application layer which imports `IRequest<T>` interface. The ideal scenario, it shouldn't view domain objects.

Least, but not last, there is the Domain, which has no references. It distills all business logic. The Domain logic exposes to application and infrastructure layer repositories, services and aggregates.

On the other hand of layers, there are the cross-cutting concerns. They were designed using the Mediatr Behavior. Those will be running before and after the execution of the command. So far there are log and validation behaviors.    