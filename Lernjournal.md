📚 Wöchentliche Lernroutine
Montag-Mittwoch: Theorie & Tutorials Donnerstag-Freitag: Praktische Implementierung Wochenende: Code Review & Refactoring

🤔 Reflexionsfragen für jede Phase
Was habe ich gelernt?
Welche Probleme bin ich angegangen?
Was würde ich anders machen?
Welche Patterns habe ich verwendet?

💡 Pro-Tipps
Führe ein Lern-Journal: Dokumentiere täglich deine Erkenntnisse
Code Daily: Auch nur 30 Minuten täglich bringen mehr als 4 Stunden am Wochenende
Teste früh und oft: Schreibe Tests parallel zu deinem Code
Suche Code Reviews: Lass anderen deinen Code anschauen

## 28.07.25
Model und Service der User Klasse, wie man es aufteilen sollte. User.cs Model Rahmenbedingung,
Interface und danach der Service welche die async Funktionen(Met6hoden) des Interfaces implementeirt

DataAnnotations, Valitation, DIncection.

Zuerst Programmieren wie ich es denke, danach Doku, Literatur, überarbeiten und dan Review.

## 29.07.25
ControllerBase wie man eifach mites [HttpPost] oder [Requred] in Cshare als Attribute setzten kann.
Die DTO auch wieder der Blueprint(Model) für die ganze Auth Flow

PasswordHasher<TUser> Klasse sollte man mitlerweile für Hashen des Password verwenden

## 30.07.25
Klare Trennung der Verantwortlichkeiten:
Komponente	Verantwortung	Beispiel
AuthController	API Endpunkte, JWT, Validation	/api/auth/login, /api/auth/register
UserService	Business Logic, CRUD	CreateUserAsync(), GetUserByIdAsync()
User Model	Datenstruktur	Properties, Validation Attributes


## 25.08.25
🎯 URLs mit deinen Namen:
🖥️ Chat UI: http://localhost:3000
🔧 Backend API: http://localhost:8080/swagger
🗄️ pgAdmin: http://localhost:8081 (mit deinen PGADMIN credentials)
📊 Database: localhost:5433 (dein bestehender Port)