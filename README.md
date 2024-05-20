## Technologies
* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [MediatR](https://github.com/jbogard/MediatR)
* [Mapster](https://github.com/MapsterMapper/Mapster)
* [FluentValidation](https://fluentvalidation.net/)
* [Elasticsearch](https://www.elastic.co/), [Serilog](https://serilog.net/), [Kibana](https://www.elastic.co/kibana)
* [Docker](https://www.docker.com/)

## Getting Started

1. Pull the Elastic Container image, to pull run `docker pull docker.elastic.co/elasticsearch/elasticsearch:8.12.0`.

2. Run the container with command `docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" -e "xpack.security.enabled=false" docker.elastic.co/elasticsearch/elasticsearch:8.12.0`.

3. Verify elastic container is running, goto chrome search bar and try visiting "localhost:9200/_aliases".

4. Edit the app.settings.json file inside the project CodingChallenge.Infrastructure.Api.
5. Run the project migrations for sql server, open the package manager console in visual studio and run `dotnet ef migrations add "CreateDb" --project src\Common\CodingChallenge.Infrastructure.SqlServer --startup-project src\Apps\CodingChallenge.Api`. You should see Migrations folder inside project CodingChallenge.Infrastructure.SqlServer
6. Run the project using IISExpress profile. Project will take some time, as it will take some time seeding the initial application data.
7. Initial run will also create default `products` index on elastic search container.
8. Download the postman collection from root of the project to test dynamic search.


## Product Search API Details `api/products/search`:

1. Api accepts json body and structure of body is below:

`{
  "query": "ultricies",
  "criterias": [
    {
      "param": "name",
      "value": "Green Angular Board 3000",
      "conjunction": "and"
    }
  ],
  "sorting": [
    {
      "field": "name",
      "order": "asc"
    }
  ],
  "pageSize": 50
}`.

2. `query` is the main search term. i.e. products will be fetched mainly based on the query string provided i.e. following fields will be scaned name, description, brand, product type.

3. `criterias` are list of objects, and each object contains 
   i. `param` name of the field criteria to be applied on. Allowed values are {name, description, brand, type}.
   ii. `value` is string which is to be searched.
   iii. `conjuction` (optional by default each object is added with or clause) are operators which can be used to define nature of link between criterias. For instance if `or` is provided current query will be concated with previous as `previous_dynamic_query or brand="Ford"`. Allowed values for `conjuction` are {"or", "and", "gt", "lt"}.

4. `sorting` are list of objects, and each object contains
    i. `field` name of the field which is to be sorted by and Allowed values are {name, description, brand, type}.
    ii. `order` is the data sorting order, Allowed values are {"asc", "desc"}.
5. To test the search api import the collection in postman `ElasticSearch.postman_collection.json` and run the `Login` request. Copy the jwt token from response body and paste into `SearchByQuery` request's Authorization section. 
6. Edit the body section with desired value and hit Send button to see the results.

## Gifs are attached in the same directory to help understand usage of API through postman collection.

### Database Configuration

The template is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update **WebApi/appsettings.json** as follows:

```json
  "DbProvider": SqlServer
```

### Multiple databases migrations
To use `dotnet-ef` for your migrations please add the following flags to your command (values assume you are executing from repository root)

* `--project src/Common/CodingChallenge.Infrastructure.{DbProvider}`
* `--startup-project src/Apps/CodingChallenge.Api`

For example, to add a new migration from the root folder:

set `"DbProvider"` in **appsettings.json** of Api project to `SqlServer`:
`dotnet ef migrations add "CreateDb" --project src\Common\CodingChallenge.Infrastructure.SqlServe --startup-project src\Apps\CodingChallenge.Api`

`dotnet ef database update --project src\Common\CodingChallenge.Infrastructure.SqlServer --startup-project src\Apps\WebApi`

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### WebApi

This layer is a web api application based on ASP.NET 6.0.x. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

### Logs

Logging into Elasticsearch using Serilog and viewing logs in Kibana.


