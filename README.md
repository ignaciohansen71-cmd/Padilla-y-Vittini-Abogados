# Padilla y Vittini Abogados

Proyecto organizado como un repositorio con frontend y backend independientes:

```text
front/    Angular 22, Bootstrap y Vitest
backend/  ASP.NET Core 10 y MailKit
```

## Desarrollo local

Instala las dependencias del frontend:

```powershell
cd front
npm install
```

Configura la contrasena de aplicacion de Google Workspace desde la raiz del repositorio. El secreto
se almacena fuera del proyecto y nunca debe incluirse en Git:

```powershell
dotnet user-secrets set "Email:Password" "TU_PASSWORD_DE_APLICACION" --project backend/PadillaVittini.Api
```

Inicia la API desde una terminal en la raiz:

```powershell
dotnet run --project backend/PadillaVittini.Api
```

Inicia Angular desde otra terminal:

```powershell
cd front
npm start
```

La aplicacion estara disponible en `http://localhost:4200` y la API en
`http://localhost:5067`. El proxy de Angular reenvia automaticamente las solicitudes `/api` al
backend durante el desarrollo.

## Verificacion

Frontend:

```powershell
cd front
npm test -- --watch=false
npm run build
```

Backend:

```powershell
dotnet build backend/PadillaVittini.Api --configuration Release
```

## Produccion

El frontend puede publicarse como sitio estatico. El backend necesita un alojamiento compatible
con ASP.NET Core 10. Configura la contrasena SMTP mediante la variable de entorno
`Email__Password`; no agregues secretos a `appsettings.json`.
