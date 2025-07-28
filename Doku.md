# Schritt-für-Schritt-Lernpfad
## Schritt 1: Grundlagen C# & ASP.NET Core
### Ziel: 
* Verstehe C#-Syntax, Klassen, Methoden, Interfaces, Dependency Injection.

### Aufgabe: 
* Erstelle eine User Klasse mit Properties und einer Validierungsmethode (Schreibe eine einfache Klasse User und eine Methode, die den Namen zurückgibt.)
* Implementiere ein Interface IUserService mit grundlegenden CRUD-Methoden
Schreibe eine Konsolenanwendung, die mit deinen Klassen arbeitet

### Doku:
* [C# Grundlagen](https://learn.microsoft.com/de-de/dotnet/csharp/tour-of-csharp/)
* [ASP.NET Core Einführung](https://learn.microsoft.com/de-de/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-9.0)
* [Dependency Injection in .NET](https://learn.microsoft.com/de-de/dotnet/core/extensions/dependency-injection)


## Schritt 2: API-Controller & Authentifizierung
### Ziel: 
* Baue REST-APIs für Registrierung und Login. (REST API Prinzipien verstehen Controller Pattern anwenden Middleware Pipeline begreifen)

### Aufgabe:
Erstelle einen AuthController mit /register und /login.(Implementiere Passwort-Hashing (z.B. mit BCrypt).
* Erstelle einen AuthController mit Register/Login Endpunkten
* Implementiere ein einfaches JWT-Token System
* Füge Swagger-Dokumentation hinzu
* Teste deine API mit Postman/Thunder Client

```csharp
// Ziel: Erstelle diese Controller-Struktur
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // TODO: Implementiere Register & Login
}
```
### Doku:
* [ASP.NET Core Web API](https://learn.microsoft.com/de-de/aspnet/core/web-api/?view=aspnetcore-9.0)
* [ASP.NET Core Web API Tutorial](https://learn.microsoft.com/de-de/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio)
* [Passwort-Hashing](https://learn.microsoft.com/de-de/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-9.0)
* [JWT Authentication](https://www.jwt.io/introduction)


## Schritt 3: Datenbankanbindung mit Entity Framework Core
### Ziel: 
* Lerne, wie du mit EF Core auf PostgreSQL zugreifst. (ORM Konzepte verstehen Code-First Migrations
Repository Pattern (optional))

### Aufgabe:
* Erstelle ein Modell Message.
* Lege eine Datenbankverbindung in appsettings.json an.
* Schreibe einen DbContext und führe Migrationen durch.
* Modelliere deine init.sql Tabellen als C# Klassen
* Erstelle einen ChatDbContext
* Generiere und führe Migrations aus
* Implementiere CRUD-Operationen

```csharp
// Aufgabe: Erweitere diese Klasse
public class User
{
    // TODO: Füge Properties basierend auf init.sql hinzu
    // TODO: Füge Data Annotations hinzu
    // TODO: Implementiere Validierung
}
```

### Doku:
* [EF Core Getting Started ](https://learn.microsoft.com/de-de/ef/core/get-started/overview/first-app?tabs=netcore-cli)
* [EF Core mit PostgreSQL](https://www.npgsql.org/efcore/?tabs=onconfiguring)


## Schritt 4: WebSocket-Integration
### Ziel: 
* Verstehe, wie du mit WebSockets in ASP.NET Core arbeitest. (WebSocket vs HTTP verstehen, Real-time Communication
Connection Management)

### Aufgabe:
* Schreibe eine Middleware, die Nachrichten broadcastet (siehe dein aktueller Code).
* Teste mit einem WebSocket-Client (z.B. Browser-Konsole).
* Füge User-Authentifizierung hinzu
* Implementiere private Nachrichten
* Speichere Nachrichten in der Datenbank

### Doku:
* [WebSockets in ASP.NET Core](https://learn.microsoft.com/de-de/aspnet/core/fundamentals/websockets?view=aspnetcore-9.0)
* [Real-time web functionality](https://learn.microsoft.com/de-de/aspnet/signalr/)


## Schritt 5: Frontend mit Angular
### Ziel: 
* Lerne Angular-Grundlagen und wie du mit dem Backend kommunizierst.
* TypeScript Grundlagen
* Angular Services & Components
* WebSocket Client Implementation

### Aufgabe:
* Setup Angular Projekt
* Erstelle Login/Register Komponenten
* Implementiere Chat Interface
* Verbinde mit WebSocket Backend

### Doku:
* Angular Tour of Heroes
* 
* [Angular Getting Started](https://angular.dev/tutorials/learn-angular)
* [Angular HTTP](https://angular.dev/guide/http)
* [WebSocket in Angular](https://developer.mozilla.org/de/docs/Web/API/WebSocket)


## Schritt 6: Docker & Deployment
### Ziel: 
* Verstehe, wie du Backend, Frontend und Datenbank mit Docker orchestrierst.

### Aufgabe:
Schreibe ein Dockerfile für das Frontend.
Passe docker-compose.yml an, damit alles zusammen läuft.

### Doku:
* [Docker für .NET](https://learn.microsoft.com/de-de/dotnet/core/docker/introduction)
* [Docker Compose](https://docs.docker.com/compose/)