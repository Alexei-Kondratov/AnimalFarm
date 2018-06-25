# AnimalFarm

## Server strucure
![Server structure](https://github.com/Alexei-Kondratov/AnimalFarm/blob/master/Server_structure.png)

## Anatomy of a service
![Service structure](https://github.com/Alexei-Kondratov/AnimalFarm/blob/master/Service_structure.png)

## Project structure
**Logic** - Business logic projects

-- **AnimalFarm.Logic.AnimalBox** - Contains AnimalBox, the class repsonsible for applying user and internal events to a specific animal.

-- **AnimalFarm.Logic.RulesetManagement** - Contains classes for working with rulesets and ruleset schedules.

**Sandbox** - Temporary sandbox project for DataAccess to AzureTables and CosmosDB

**ServiceTypes** - Contains definitions for Service Fabric services.

-- **AnimalFarm.AdminService** - Stateless service that handles adminsitrative commands like clearing caching and data reset.

-- **AnimalFarm.AnimalFarm** - Stateful service that maintains a cache for animals data and handles animal queries and events.

-- **AnimalFarm.AuthenticationService** - Stateless service that handles user login/password authentication by isuuing JWT tokens.

-- **AnimalFarm.Gateway** - Stateless service that handles all incoming requests and redirects them to other services.

-- **AnimalFarm.Ruleset** - Stateful service that maintains a cache for rulesets and provides rulesets and rulesets schedule data to users and other services.

-- **AnimalFarm.Service** - Contains common logic for services.

**Tests** - Contains unit and integration tests for other projects.

**Tools**

-- **AnimalFarm.Tools.ResetData** - Console applciation for resetiing database structure, business data and configuration.

**AnimalFarm.Data** - Contains base classes for data access: DataSources, Repositories and Transactions.

**AnimalFarm.Data.Seed** - Contains the logic for seeding db with a default data and the default data itself.

**AnimalFarm.Model** - Contains POCO classed for describing model entities like animals and rulesets.

**AnimalFarm.Service** - Defines the Service Fabric application and applciation-level configuration.

**AnimalFarm.Service.Utils** - Contains utility classes that rely on Service Fabric and ASP.NET libraries (e.g. middleware, intraservice communication etc.)

**AnimalFarm.Utils** - Contains base utility classes (e.g. for configuration and dependency injection(.

**AnimalFarm.WebUI** - A simple test harness implemented as a React SPA. **Currently hosted at http://animalfarm.azurewebsites.net/**
