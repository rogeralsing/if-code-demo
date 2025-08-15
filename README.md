# Reflections from author

Tools used: ChatGPT "Codex"

This complete codebase was generated using AI tooling.
0 lines of manual code, and 6 prompts in total.
All prompts are available in the `Prompts` folder.
Prompt 1-3 Creates the services and the tests.
Prompt 4-6 is mostly clean up and cosmetics.

The project was set up with a .NET solution and two empty Asp.NET Core Web API projects.

Everything beyond that is generated code.

Since this specific task is about showcasing how one can work effectively using AI tooling, I went for a minimalistic approach.
AI tooling tends to perform better with code within a file, rather than scattered across multiple files.

I´m personally a believer of "disposable code", --true microservices--, code that you can throw away and replace when needed.

The code should still be easy to read and understand, but this approach gives more flexibility to what you allow where.
You might not need DTO, Business Entity, and Data Entity models for every single thing you interact with.

Basically, less cermony, just code that works.


# ThreadPilot Integration Demo

This repo hosts two minimal REST APIs that expose data from a fictional core system.

## Vehicle API
`GET /vehicles/{registrationNumber}` returns basic vehicle information.

## Insurance API
`GET /insurances/{personalNumber}` returns insurances for a person. 
Any car insurance includes vehicle details fetched by calling the Vehicle API. 

## Running
```bash
dotnet run --project TpVehicleAPI
# in another terminal
dotnet run --project TpInsuranceAPI
```
The insurance service expects the vehicle service at the URL in
`VehicleApiBaseUrl` (default `http://localhost:5005`).

## Testing
```bash
dotnet test
```

## Architecture
- Both services use minimal APIs in a single `Program.cs` file.
- Data comes from interfaces (`IVehicleDataProvider` and `IInsuranceDataProvider`) with hardcoded implementations for simplicity.
- Insurance service calls the vehicle service through an `IVehicleClient` abstraction.

## Error handling
- Inputs are validated with data annotations; invalid values yield `400 Bad Request`. this is not a solid full validation, but a minimal check, that allows developers to more easily understand API requirements.
- Unknown identifiers result in `404 Not Found`.
- Api to Api calls are handled with `HttpClient` wrapped in Polly
  for basic retry logic.


## Extensibility
- Using inprocess tests for entire APIs allow us to easily replace dependencies, e.g. with mocks or hardcoded data.
- Additional endpoints can follow the same minimal style.

## Security considerations
- No authentication or authorisation is implemented.
- HTTPS and proper input sanitisation would be required for production use.
- Basic rate limiting is enabled to reduce the impact of potential abuse.

## What is not included

### Swedish SSN parsing and validation

Swedish SSN parsing and validation could be an entire code test on its own.

- e.g. with or without dashes, length, checksum, etc.
- Samordningsnummer (coordination number) vs personnummer (personal number) handling.
- Validating Personnummer vs Organisationsnummer

### Vehicle registration number parsing and validation
Vehicle registration number parsing and validation could also be an entire code test on its own.
- Rules depend on the year it was issued.
- There are lists of forbidden character combinations, AFAIK also depending on the year.

### Proper unified error handling

Error handling could be done in many different ways.
Result Types, Railway oriented programming, Error codes vs exceptions.

For a minimal code demo, such design could easily outshadow the main purpose of the code test.

### Performance and concurrency

If the actual system were to interface with e.g. older mainframe systems.
Interactions would likely have to be bounded in concurrency limitations.
e.g. only max 10 concurrent calls to the mainframe at a time.
Which can be easily enforced from within a single process using semaphores, less so if running multiple processes.
You likely end up with a queue and bounded worker pool to guarantee this limitation at scale.

REST to REST tends to be temporally coupled, this may or may not be a problem in this case.
If Vehicle API is down, then so is Insurance API.

Serialization formats and wire protocols,
JSON over HTTP is a common choice, but not the only one.
gRPC + HTTP proxy is another variant, use gRPC for internal services and HTTP for external, with no extra code needed.
