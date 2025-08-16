# Agents

This repository contains a coding assignment for building an integration layer between a new core system (ThreadPilot) and legacy systems.

## Required functionality
- Two separate REST API projects.
  - **Endpoint 1:** accepts a vehicle registration number and returns vehicle information.
  - **Endpoint 2:** accepts a personal identification number and returns the person's insurances with monthly costs.
    - Include vehicle info for any car insurance by calling Endpoint 1.
- Document APIs with Swagger using Swashbuckle.
- Provide graceful error handling for invalid input or missing data.
- Supply at least three unit tests for key logic.
- Include a README that explains architecture, how to run and test locally, and thoughts on error handling, extensibility, security, and a short personal reflection.

## Coding guidelines
- All projects must target **.NET 9.0**. Do not change the framework version.
- Keep code pragmatic and minimal; favour readability similar to Go over heavy patterns.
- Avoid reflection where possible.
- Use comments to clarify code and tests when helpful, but keep them concise.
- Ask before breaking public API definitions.
- Optimise code so AI agents can understand and modify it easily.

