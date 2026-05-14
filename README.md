# 🚀 Shva Global Transaction Simulator (Backend)

A professional .NET 8 Web API built to simulate and validate global financial transactions. This system determines whether a transaction should be "Approved" or "Rejected" by calculating business hours across different global Time Zones.

---

## 🏗️ System Architecture

The project follows a clean, layered architecture:
- **API Layer**: Handles HTTP requests, JSON serialization, and CORS security.
- **Domain/Core**: Contains Business Logic, Entities, and DTOs.
- **Infrastructure**: Managed via Entity Framework Core with dynamic schema mapping.

---

## ⚙️ Configuration (`appsettings.json`)

To ensure flexibility, all environment-specific settings are moved out of the code and into the configuration file:

- **DatabaseConfig**: Allows dynamic renaming of Tables and DB Schemas without code changes.
- **BankPolicy**: Defines default business hours (e.g., `09:00:00` to `18:00:00`) and weekend rules.
- **CorsConfig**: Manages security by allowing specific Frontend Origins (e.g., your React local dev server).
- **PagingConfig**: Controls how many transactions are displayed in the history log.

---

## 🗄️ Database & Migrations

The system uses **Entity Framework Core** with a code-first approach. The database schema is dynamically driven by the `appsettings.json` file.

### 🛠️ Commands to Run Migrations

Depending on your development environment, use one of the following sets of commands to initialize your database:

#### Option A: Using Terminal / CLI (VS Code / Rider / CMD)
1. **Create the Migration**:
   ```bash
   dotnet ef migrations add InitialCreate
#### 🧪 API Testing with `.http` File

The project includes a dedicated `test-api.http` file. This is a modern, lightweight alternative to Postman.