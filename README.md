# Viva Assessment

**Framework:** .NET 8.0 | **Language:** C# | **Database:** SQL Server
**Architecture:** Clean Architecture

-------

## Overview
The project consists of four incremental tasks:

1. **Compute the second largest integer** from an input array.
2. **Retrieve country data** from the public RestCountries API.
3. **Persist the retrieved data** into a SQL Server database.
4. **Implement a caching layer** to improve response performance.

-------

## Architectural Style
The solution follows **Clean Architecture** principles — ensuring separation of concerns, high testability, and maintainability.  
It is structured into four layers:

- **VivaAssessment.Api** – Presentation layer (controllers, Swagger, DI configuration).  
- **VivaAssessment.Application** – Application layer (services, abstractions, DTOs).  
- **VivaAssessment.Domain** – Core domain models (entities such as `Country`, `RequestObj`).  
- **VivaAssessment.Infrastructure** – Infrastructure layer (persistence, caching, HTTP clients).

-------

## How to Run

1. Open the solution in **Visual Studio 2022**.
2. Set `VivaAssessment.Api` as the **Startup Project**.
3. Update `appsettings.json` with your **SQL Server connection string**.
4. Run the project (`Ctrl + F5`) (navigates to `/swagger`).

-------

## Database Setup

```sql
TRUNCATE TABLE dbo.CountryBorders;
DELETE FROM dbo.Countries;
