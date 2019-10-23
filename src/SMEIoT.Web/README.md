## User secrets

To build connection strings, we need to seed some secrets for dotnet.

```bash
dotnet user-secrets list
dotnet user-secrets set ConnectionStrings:User smeiot
dotnet user-secrets set ConnectionStrings:Password smeiot
```

## Database Management

We use EntityFramework Core.

Command line installation:
```
dotnet tool install --global dotnet-ef
```

Then update the database:

```
dotnet ef database update
```

## Environment

We uses environments for some configurations. To switch it, you can set env by bash. On Windows (Powershell), use `$env:SMEIOT_ENVIRONMENT='Development'` to set `DCRAWL_ENVIRONMENT` with `Development` for example.
