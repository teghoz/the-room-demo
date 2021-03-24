# the-room-demo

Instructions [here](Question.md)


## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) and [configured](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Windows-setup#configure-docker) docker in your environment. After that, you can run the below commands from the [**/src/**](/src) directory and get started with the `TheRoomContainers` immediately.

```powershell
docker-compose build
docker-compose up
```

### Project Structure
I have approached the problem in both a [microservices](/src) and [monolithic](/TheRoomSimpleAPI) way.

### Monolithic Approach
Once the container is loaded up, you can get access to the [`TheRoomSimpleAPI`](/TheRoomSimpleAPI) service. It is standalone and performs all its activity itself. You would have to use the `/api/v1/Auth/Register` endpoint to get registered and logged in. An unthentication token would be provided in the response which you would use to access the other endpoints. It has its swagger documentation so you would have a means to see all the available options and test it directly.

### Microservices Approach
In this approach I have splitted out the concerns into different isolated services. The following are the services:

- [Identity Server](/src/Services/Identity). Find out more [here](https://github.com/IdentityServer/IdentityServer4) and [here](https://identityserver4.readthedocs.io/en/latest/)

- [ServiceList Service](/src/Services/ServiceList) - Which just handles the list of all services.
- [Bonus Service](/src/Services/Bonus)
- [RabbitMQ](https://github.com/rabbitmq/rabbitmq-dotnet-client/) - which servers as the message broker on development. 
*Do note that you have to register a new user in order to use the functionality*

- [Web Status](https://github.com/xabaril/AspNetCore.Diagnostics.HealthChecks) - Which polls the services to report health checks.
- SQL Server - which houses some of the sevices databases.
- [MongoDb](https://docs.mongodb.com/manual/) - which the bonus service would use.
- [Seq](https://datalust.co/seq) - which handles logging

#### Frontend Client - React
- [React](src/Web/the-room) - The react app gets authorized by the identity server and stores its token in `localStorage`. With that it gets access to the  [ServiceList Service](/src/Services/ServiceList) which provides information about the service lists. 


##### Auth service in action
![](/Images/auth-service-in-action.png)

##### React app

![](/Images/react-app.png)

*Do note that i did not spend alot of time on the aesthetics as a result of time constraint*

I have also showcased [Unit Testing](/src/Services/ServiceList/tests/ServiceList.UnitTests) and [Integration Testing](/src/Services/ServiceList/tests/ServiceList.FunctionalTests) for the ServiceListAPI Services.

---

### Questions
#### How do you document code?
Although some on the development side of the aisle would argue that good documentation would lead to job insurity; LOL, Good Documentation is key especially when speed to market and scalability is of utmost importance. Imagine a new resource joining and that person has to navigate through the existing code base for example. 

Markdowns are one way i achieve this. There are other tools like confluence my current organization uses. One really easy way to also do this is to stick with conventions. Once everyone is familiar with a convention then you have already documented your code without typing extra information.

#### What are your thoughts on unit testing?
As systems begin to scale, the introduction of new code has the potential to introduce side effects. Side effects that one might not be thinking about at that time. Unit testing helps reduce regressions. 

Furthermore, it fosters code quality assurance. From my experience, my best benefit is when debugging code. It accelerates the process of getting to the bottom of a potential issue. 

Finally, unit testing and TDD as a whole has its challenges - for example it subject to the develop's scope of understand as regards to side effects and usability, and so i think BDD is the way to move forward.

Developers are built to solve problems while QAs are built to find problems, that mismatch always causes problems and it affects howe well developers write tests. So if we start from the behaviours and walk backwards to the problems, it reduces unneccesary back and forth with QAs, increases productivity and code quality.

#### What design pattern have you used in your project?
#### What are the most important performance issues in ASP.Net web applications?

#### What do you think about Typescript?
Well, since i come from a .NET and static typing background, I think very highly of it. Although it can be annoying atimes when using it especially for those who come from an older ESMASCRIPT ES3 > version where you had the freedoms to code dynamically.

However, for large codebases, you want a way to catch issues, errors and exceptions at compile time. Typescript provides you that luxury. Additionally it allows for deeper use of OOP concepts

#### What are the most important performance issues in React web applications?
Well, one issue that i have experience is bad architecture of components. This leads to prop drooling and makes user experience bad as objects keep moving up and down the tree. 

There is always the question you need to ask everytime, when is large *LARGE*. The idea behind componenting is reuse but it also to help with readability too. If a component is too large that may be it should be further broken down also regardless of whether that component would be reused or not.



