# AzBina

AzBina artıq həm **API**, həm də **sadə web frontend** ilə işləyir.
Frontend `src/API/wwwroot` içindədir və API ilə eyni host/port üzərindən serv olunur.

## Lokal run

```bash
dotnet restore
dotnet build AzBina.sln
dotnet run --project src/API/API.csproj
```

Brauzerdə açın:

- `https://localhost:5001` və ya launchSettings-də göstərilən URL
- Swagger: `https://localhost:5001/swagger` (Development mühitində)

## Frontend-də nələr var?

- Login formu (`/api/Auth/login`, `login` field-i username və ya email qəbul edir)
- City + District listəsi (`/api/City`, `/api/District`)
- Property listing görüntüləmə (`/api/PropertyListing`)
- Yeni property yaratma formu (`/api/PropertyListing`, Bearer token ilə)

## Production-a çıxarmaq üçün qısa plan

1. `ASPNETCORE_ENVIRONMENT=Production` ilə run edin.
2. SQL Server connection string-i production DB ilə əvəz edin.
3. JWT, MinIO və Email konfigurasiya dəyərlərini secrets/env vars ilə verin.
4. Reverse proxy (Nginx/IIS) arxasında HTTPS aktiv edin.
5. `dotnet publish -c Release` ilə build artifact yaradın.

Nümunə publish:

```bash
dotnet publish src/API/API.csproj -c Release -o ./publish
```

Sonra `./publish/API` binary-ni serverdə run edə bilərsiniz.


### Swagger açılırsa (troubleshooting)

Əgər yenə Swagger açılırsa, adətən IDE köhnə profile/url saxlayır:

1. Run profile olaraq `http` və ya `https` seçin (Project profile).
2. Browserdə birbaşa `http://localhost:5197/index.html` və ya `https://localhost:7273/index.html` açın.
3. Visual Studio istifadə edirsinizsə, stop edib yenidən run edin (gərəkirsə `.vs` cache-i təmizləyin).
4. Swagger yenə lazımdırsa əl ilə `/swagger` yazın.
