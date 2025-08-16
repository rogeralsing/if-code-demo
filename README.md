# Reflections from the Author

**Tools used:** ChatGPT "Codex"

This entire codebase was generated using AI tooling — **0 lines of manual code**, and **9 prompts** in total.

Could this have been done faster by combining AI and manual coding? **Absolutely.**  
But my goal was to showcase that the right AI tooling, combined with tests and requirements, can produce a complete, runnable, and testable codebase with minimal manual effort.

> Any similar project or experience you’ve had in the past.

**Real-World Examples**

For real-world examples where I apply this approach, see:

- [protoactor-go PRs (Codex)](https://github.com/asynkron/protoactor-go/pulls?q=label%3Acodex)  
- [protoactor-dotnet PRs (Codex)](https://github.com/asynkron/protoactor-dotnet/pulls?q=label%3Acodex)  
- [protoactor-website PRs (Codex)](https://github.com/asynkron/protoactor-website/pulls?q=label%3Acodex)  

Companies like **ByteDance** (creators of TikTok) use ProtoActor Go for internal services, and organizations like **Beamable, ChargeAmps, and ABAX** use ProtoActor .NET for their backend services.

Another real world usecase was CAB - Car repair management system for the insurance industry.

Their internal teams stated that their old legacy systems could not be ported to .NET Core, due to outdated dependencies and libraries and complex codebase.

I created various CLI tools using AI generated code to help them migrate their codebase to .NET Core.

e.g. 
* Automatically migrate from NHibernate to Entity Framework Core
* Automatically migrate from Typed DataSets to Entity Framework Core
* Automatically migrate from LinqToSql to Entity Framework Core
* Automatically apply Dependency Injection via ctors.
* Automatically replace "new" or static dependencies with Dependency Injection

> What was challenging or interesting in this assignment.

I simply enjoy this kind of work. after writing code manually for 30+ years, I find it fascinating to see how AI can generate code that is not only functional but also maintainable and testable.

> What you would improve or extend if you had more time.

Plenty. it´s a demo. Not having business rules in endpoints, better error propagation. e.g. result types. 
In a real system one would also have real Units to test. not just endpoints interacting with some downstream data service.

Swedish SSN and reg plates are a solved problem, there are libraries and lookup tables for that. this was omitted on purpose to keep the codebase minimalistic.

---

### Prompt Breakdown
All prompts are available in the [Prompts](/Prompts/) folder:

- **Prompts 1–4**: created services and tests  
- **Prompts 5–6**: mostly code review feedback  
- **Prompts 7–9**: cleanup — removing unused files, usings, etc.  

![prompts](/Prompts/prompts.png)

With clearer requirements upfront (e.g. validation, error handling, retry policies), this could likely have been reduced to half the number of prompts. Some sub-prompts were just me fighting Codex changing the .NET version.

---

### About the Codebase
The project started as a .NET solution with two empty ASP.NET Core Web API projects.  
Everything beyond that is **AI-generated code**.

Since this task was about demonstrating effective AI-assisted workflows, I went for a **minimalistic structure**. AI tooling generally performs better with code consolidated in fewer files. Tools like CodePilot, Cursor AI, and Windsurf still struggle with fragmented projects, whereas Codex does much better (though slower, since it runs tests, searches, and iterates extensively).

---

### Philosophy
I’m a believer in **disposable code** — true microservices you can throw away and replace when needed.  

The code should still be easy to read and understand, but less ceremony (DTOs, BE, DE models everywhere) often means more flexibility and faster iteration.  

Basically: **less ceremony, just working code.**

---

# ThreadPilot Integration Demo

This repo hosts two minimal REST APIs that expose data from a fictional core system.

## Vehicle API
`GET /api/v1/vehicles/{registrationNumber}` returns basic vehicle information.

## Insurance API
`GET /api/v1/insurances/{personalNumber}` returns insurances for a person.  
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
- The insurance service calls the vehicle service through an `IVehicleClient` abstraction.

## Error Handling
- Inputs are validated with data annotations; invalid values yield `400 Bad Request`. This is not a solid full validation, but a minimal check that allows developers to more easily understand API requirements.
- Unknown identifiers result in `404 Not Found`.
- Downstream service errors yield `503 Service Unavailable`.
- API-to-API calls are handled with `HttpClient`, wrapped in Polly for basic retry logic.

## Extensibility
- Using in-process tests for entire APIs allows us to easily replace dependencies, e.g. with mocks or hardcoded data.
- Additional endpoints can follow the same minimal style.
- The specification lacks real requirements in terms of business rules, e.g. what are viable insurance combinations. In a real-world app, insurance combinations would likely not be verified in the endpoint itself.

## Security Considerations
- No authentication or authorization is implemented.
- HTTPS and proper input sanitization would be required for production use.
- Basic rate limiting is enabled to reduce the impact of potential abuse.

## Schema Evolution
There are many ways to handle schema evolution. I went with the simplest approach of just versioning endpoints.  
This is by far the easiest for any AI tooling to understand and implement.  
Just slap more endpoints on the existing API, and inform end consumers when older endpoints will become deprecated.  
This also works very well with Swagger. There is no dynamic aspect to the data itself.

---

## What Is Not Included

### Swedish SSN Parsing and Validation
Swedish SSN parsing and validation could be an entire code test on its own.

- With or without dashes, with or without century, length, checksum, etc.
- Samordningsnummer vs. personnummer vs. organisationsnummer.

### Vehicle Registration Number Parsing and Validation
Vehicle registration number parsing and validation could also be an entire code test on its own.
- Rules depend on the year it was issued.
- There are lists of forbidden character combinations, also depending on the year.

### Proper Unified Error Handling
Error handling could be done in many different ways:  
Result types, railway-oriented programming, error codes vs. exceptions.

For a minimal code demo, such design could easily overshadow the main purpose of the code test. Throwing and returning HTTP information is sufficient for this demo.

### Performance and Concurrency
If the actual system were to interface with, e.g., older mainframe systems, interactions would likely have to be bounded by concurrency limitations.  
For example, only a maximum of 10 concurrent calls to the mainframe at a time.  
This can be easily enforced from within a single process using semaphores, less so if running multiple processes.  
You likely end up with a queue and bounded worker pool to guarantee this limitation at scale.

REST-to-REST tends to be temporally coupled; this may or may not be a problem in this case.  
If the Vehicle API is down, then so is the Insurance API.

Serialization formats and wire protocols:  
JSON over HTTP is a common choice, but not the only one.  
gRPC + HTTP proxy is another variant: use gRPC for internal services and HTTP for external, with no extra code needed.

---

**TL;DR**: It might not be super pretty, but it does show how entire systems could be built with less effort than traditional methods — without any AI hallucinations and frequent rollbacks, which tools like Cursor AI tend to produce.
