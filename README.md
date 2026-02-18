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

- İlk açılışda yalnız Login ekranı görünür.
- Daxil olduqdan sonra solda modul menyusu açılır: `City`, `District`, `Elanlar`.
- Hər modulda list + create + update + delete əməliyyatları UI-dan idarə olunur.
- Login endpoint-i `/api/Auth/login` üçün `login` (username/email) və `password` göndərilir.
- Qeyd: `City` və bəzi `Elan` əməliyyatları backend policy səbəbi ilə admin icazəsi tələb edə bilər.

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


## Admin seed (Development)

- Development mühitində app start olanda `Seed` konfiqurasiyasındakı admin istifadəçisi yoxdursa yenidən yaradılır.
- Hazırkı default: `admin@mail.com` / `Admin123!`.
- Admini silsəniz, app-i yenidən başladanda seed yenidən düşəcək.
