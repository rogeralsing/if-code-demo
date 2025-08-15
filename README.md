# ThreadPilot Integration Demo

This repo hosts two minimal REST APIs that expose data from a fictional core system.

## Vehicle API
`GET /vehicles/{registrationNumber}` returns basic vehicle information.

## Insurance API
`GET /insurances/{personalNumber}` returns insurances for a person. Any car insurance
includes vehicle details fetched by calling the Vehicle API. Prices are fixed per
type: pet $10, personal health $20, car $30.

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
- Data comes from interfaces (`IVehicleDataProvider`, `IInsuranceDataProvider`) with
  hardcoded implementations for simplicity.
- Insurance service calls the vehicle service through an `IVehicleClient` abstraction.

## Error handling
- Inputs are validated with data annotations; invalid values yield `400 Bad Request`.
- Unknown identifiers result in `404 Not Found`.
- Missing vehicle information on a car insurance leaves the `vehicle` field empty.

## Extensibility
- Real data providers or HTTP clients can replace the hardcoded ones without touching
  the endpoint logic.
- Additional endpoints can follow the same minimal style.

## Security considerations
- No authentication or authorisation is implemented.
- HTTPS and proper input sanitisation would be required for production use.

## Reflection
Keeping the code compact and explicit makes it easy to read and modify. Abstracting
external dependencies behind interfaces should allow incremental growth without
large refactors.
