# BeFit

Prosta aplikacja internetowa do zarządzania treningami, stworzona w **ASP.NET Core MVC (.NET 10)** w języku C#.

## Funkcjonalności

- Rejestracja i logowanie użytkowników (ASP.NET Core Identity)
- Zarządzanie sesjami treningowymi i wykonanymi ćwiczeniami (CRUD)
- Katalog typów ćwiczeń zarządzany przez administratora
- Dostęp użytkowników wyłącznie do własnych danych
- Statystyki treningowe z ostatnich 28 dni

## Technologie

- C# / ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Core Identity
- Bootstrap

## Uruchomienie

1. Otwórz projekt w Visual Studio.
2. Skonfiguruj połączenie z bazą danych w `appsettings.json`.
3. Wykonaj `Update-Database` w konsoli menedżera pakietów NuGet.
4. Uruchom aplikację (`Ctrl + F5`).

Projekt wykonany w ramach zajęć z programowania aplikacji internetowych.