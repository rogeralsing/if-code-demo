# Reflections from the author

**Tools used:** ChatGPT "Codex"

This entire codebase was generated using AI tooling — **0 lines of manual code**, and **9 prompts** in total.

Could this have been done faster by combining AI and manual coding? **Absolutely.**  
But my goal was to showcase that the right AI tooling, combined with tests and requirements, can produce a complete, runnable, and testable codebase with minimal manual effort.

---

### Prompt breakdown
All prompts are available in the [Prompts](/Prompts/) folder:

- **Prompts 1–3**: created services and tests  
- **Prompts 4–6**: mostly code review feedback  
- **Prompts 7–9**: cleanup — removing unused files, usings, etc.  

With clearer requirements upfront (e.g. validation, error handling, retry policies), this could likely have been reduced to half the number of prompts. Some sub-prompts were just me fighting Codex changing the .NET version.

---

### About the codebase
The project started as a .NET solution with two empty ASP.NET Core Web API projects.  
Everything beyond that is **AI-generated code**.

Since this task was about demonstrating effective AI-assisted workflows, I went for a **minimalistic structure**. AI tooling generally performs better with code consolidated in fewer files. Tools like CodePilot, Cursor AI, and Windsurf still struggle with fragmented projects, whereas Codex does much better (though slower, since it runs tests, searches, and iterates extensively).

---

### Philosophy
I’m a believer in **disposable code** — true microservices you can throw away and replace when needed.  

The code should still be easy to read and understand, but less ceremony (DTOs, BE, DE models everywhere) often means more flexibility and faster iteration.  

Basically: **less ceremony, just working code.**

---

### Real-world examples
For real-world examples of this approach, see:

- [protoactor-go PRs (Codex)](https://github.com/asynkron/protoactor-go/pulls?q=label%3Acodex)  
- [protoactor-dotnet PRs (Codex)](https://github.com/asynkron/protoactor-dotnet/pulls?q=label%3Acodex)  
- [protoactor-website PRs (Codex)](https://github.com/asynkron/protoactor-website/pulls?q=label%3Acodex)  

Companies like **ByteDance** (creators of TikTok) use ProtoActor Go for internal services, and organizations like **Beamable, ChargeAmps, and ABAX** use ProtoActor .NET for their backend services.



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
