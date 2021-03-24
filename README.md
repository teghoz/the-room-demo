# the-room-demo

Instructions and Questions [here](Question.md).

Here are my [answers](Answers.md) :ok_hand:.


## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) and [configured](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Windows-setup#configure-docker) docker in your environment. After that, you can run the below commands from the [**/src/**](/src) directory and get started with the `TheRoomContainers` immediately.

```powershell
docker-compose build
docker-compose up
```

### Project Structure
I have approached the problem in both a [microservices](/src) and [monolithic](/TheRoomSimpleAPI) way.

### Monolithic Approach
Once the container is loaded up :rocket:, you can get access to the [`TheRoomSimpleAPI`](/TheRoomSimpleAPI) service. It is standalone and performs all its activity itself. You would have to use the `/api/v1/Auth/Register` endpoint to get registered and logged in. An unthentication token would be provided in the response which you would use to access the other endpoints. It has its swagger documentation so you would have a means to see all the available options and test it directly.

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

##### React app
![](/Images/react-app.png)

##### Auth service in action
![](/Images/auth-service-in-action.png)

##### ServiceListAPI Swagger
![](/Images/service-list-swagger.png)

##### ServiceListAPI Sample Request
![](/Images/end-point-in-action.png)

*Do note that i did not spend alot of time on the aesthetics as a result of time constraint*

#### Testing
I have also showcased [Unit Testing](/src/Services/ServiceList/tests/ServiceList.UnitTests) and [Integration Testing](/src/Services/ServiceList/tests/ServiceList.FunctionalTests) for the ServiceListAPI Services.

I have use [AutoFixture](https://github.com/AutoFixture/AutoFixture) to mock sample data on the [Unit Test](/src/Services/ServiceList/tests/ServiceList.UnitTests) and the awesome [.Net TestHost](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.testhost?view=aspnetcore-5.0) for integration testing.



