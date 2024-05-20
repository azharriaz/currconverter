## Technologies
* [ASP.NET Core 8](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0)
* [MediatR](https://github.com/jbogard/MediatR)
* [Mapster](https://github.com/MapsterMapper/Mapster)
* [FluentValidation](https://fluentvalidation.net/)
* [Serilog](https://serilog.net/))

## Running the Application

1. Clone this repository to your local machine using git clone.

2. Open the solution in your preferred IDE (e.g., Visual Studio, Visual Studio Code).

3. Ensure that the required NuGet packages are restored.

4. Application settings are already configured to use the https://api.frankfurter.app/.

5. Build the solution to ensure all projects are compiled successfully.

6. Run the application using the IISExpress profile or by executing dotnet run in the terminal/command prompt from the root of the solution.

7. Once the application is running, you can access the following endpoints:

# Retrieve Latest Exchange Rates:

7.1 Endpoint: GET /currencies/latest?baseCurrency={baseCurrency}
    Example: GET http://localhost:5000/currencies/latest?baseCurrency=EUR

# Convert Currency:

7.2 Endpoint: POST /currencies/convert
    Example: Send a POST request with the conversion details in the request body.
	`{
		"amount": 10,
		"fromCurrency": "GBP",
		"toCurrency": "USD"
    }`

# Get Historical Rates:

7.3 Endpoint: POST /currencies/history
    Example: Send a POST request with the historical rates query details in the request body.
	`{
		"baseCurrency": "USD",
		"startDate": "2023-01-01",
		"endDate": "2023-01-31",
		"pageNumber": 1,
		"pageSize": 10
	}`

## Postman collection is attached in the same directory to help understand usage of API.

## Running Tests
1. Navigate to the test project directory (CodingChallenge.UnitTests) in your terminal/command prompt.

2. Execute dotnet test to run all unit tests.

3. Review the test results to ensure all tests pass successfully.

4. Note: Tests are only covered for first two apis (latest, convert) and pending for Historical Exchange Rate api.

```

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### WebApi

This layer is a web api application based on ASP.NET 8.0.x. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Program.cs* should reference Infrastructure.

=======