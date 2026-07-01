# Silph Console Tools - Inheritor Project Setup

## Overview

The Silph Console application now provides comprehensive tools to help developers set up new projects that inherit from the Silph framework. These tools automate the creation of configuration files, directory structures, and database scaffolding.

> **📘 Working with Existing Projects?**  
> This guide focuses on **new projects**. If you're integrating Silph into an **existing project**, see [Retrofitting Existing Projects](RETROFITTING_EXISTING_PROJECTS.md) for detailed guidance on adapting these tools to work with established codebases.

## Installation & Usage

Build the Silph.Console project and run it using:
```bash
dotnet run --project Silph.Console -- <command> [options]
```

Or install it globally (future enhancement) and use:
```bash
silph <command> [options]
```

## Available Commands

### 1. `silph init` - Project Initialization

**Purpose**: Creates the initial `silph.config.json` configuration file for a new inheritor project.

**Usage**:
```bash
silph init [--output <path>]
```

**Interactive Prompts**:
- **Project Name**: Required. The name of your project (e.g., "MyApp", "Pokedex")
- **Project ID**: Optional. A unique identifier for the project. If not provided, a random 8-character hex ID is generated.

**Output**:
- Creates `silph.config.json` in the current directory (or specified output path)
- Contains default configuration for:
  - Project metadata (ID, Name, Version)
  - Database settings (provider, connection string, scaffolding paths)
  - Silph infrastructure database settings
  - Logging configuration

**Example Configuration Generated**:
```json
{
  "project": {
	"id": "db07d9c4",
	"name": "MyTestApp",
	"version": "0.1.0"
  },
  "database": {
	"provider": "SqlServer",
	"connectionStringName": "MYTESTAPP_CONNECTION_STRING",
	"dataProjectPath": "MyTestApp.Data/MyTestApp.Data.csproj",
	"dbContextName": "MyTestAppDbContext",
	"scaffoldOutputDirectory": "Models"
  },
  "silphDatabase": {
	"enabled": true,
	"connectionStringName": "SILPH_CONNECTION_STRING",
	"registerProject": true
  },
  "logging": {
	"structureEnabled": true,
	"bootstrapEnabled": true,
	"maxLogRecords": 1000
  }
}
```

---

### 2. `silph db setup` - Database Configuration

**Purpose**: Configures database settings, creates directory structure for the data project, and prepares for scaffolding.

**Usage**:
```bash
silph db setup
```

**Prerequisites**:
- `silph.config.json` must exist (run `silph init` first)

**Interactive Prompts**:
1. **Database Provider**: Choose from SqlServer, PostgreSQL, SQLite, or MySQL (defaults to current setting)
2. **Create Data Project Directory**: Confirms creation of the data project folder and Models subdirectory
3. **DbContext Name**: Name for the Entity Framework DbContext class
4. **Connection String Variable Name**: Environment variable name for the database connection string

**Output**:
- Creates directory structure: `{ProjectName}.Data/Models/`
- Updates `silph.config.json` with new database settings

**Next Steps Provided**:
1. Set the connection string environment variable
2. Install EF Core tools: `dotnet tool install --global dotnet-ef`
3. Run scaffolding: `silph db scaffold`

---

### 3. `silph db scaffold` - Database Scaffolding

**Purpose**: Runs Entity Framework Core scaffolding to generate DbContext and entity models from an existing database.

**Usage**:
```bash
silph db scaffold
```

**Prerequisites**:
- `silph.config.json` must exist
- Data project directory must be created (via `silph db setup`)
- Database connection string must be set as an environment variable OR provided manually when prompted
- `dotnet-ef` tools must be installed globally

**Process**:
1. Reads configuration from `silph.config.json`
2. Retrieves connection string from the configured environment variable
3. Builds and executes the appropriate `dotnet ef dbcontext scaffold` command based on provider
4. Generates DbContext and entity models in the configured output directory

**Provider Packages Used**:
- **SqlServer**: `Microsoft.EntityFrameworkCore.SqlServer`
- **PostgreSQL**: `Npgsql.EntityFrameworkCore.PostgreSQL`
- **SQLite**: `Microsoft.EntityFrameworkCore.Sqlite`
- **MySQL**: `Pomelo.EntityFrameworkCore.MySql`

**Example Command Generated**:
```bash
dotnet ef dbcontext scaffold "<connection-string>" Microsoft.EntityFrameworkCore.SqlServer \
  --project "MyTestApp.Data" \
  --output-dir Models \
  --context MyTestAppDbContext \
  --force
```

---

### 4. `silph db info` - Database Configuration Display

**Purpose**: Displays the current database configuration settings from `silph.config.json`.

**Usage**:
```bash
silph db info
```

**Output Example**:
```
Database Configuration
=====================

  Provider:              SqlServer
  Connection String Var: MYTESTAPP_CONNECTION_STRING
  Data Project Path:     MyTestApp.Data/MyTestApp.Data.csproj
  DbContext Name:        MyTestAppDbContext
  Scaffold Output:       Models

  ✓ Connection string configured
```

---

## Typical Workflow

### Starting a New Inheritor Project

1. **Initialize the project**:
   ```bash
   cd MyNewProject
   silph init
   # Enter project name when prompted
   ```

2. **Review and customize** `silph.config.json` as needed

3. **Set up database configuration**:
   ```bash
   silph db setup
   # Follow prompts to configure database settings
   ```

4. **Set connection string environment variable**:
   ```bash
   # PowerShell
   $env:MYTESTAPP_CONNECTION_STRING = "Server=localhost;Database=MyDb;..."

   # Or set it system-wide through Environment Variables UI
   ```

5. **Install EF Core tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

6. **Run scaffolding**:
   ```bash
   silph db scaffold
   ```

7. **Verify generated models**:
   - Check `{ProjectName}.Data/Models/` for entity classes
   - Review the generated DbContext

---

## Configuration File Structure

### Project Section
- `id`: Unique identifier for the project
- `name`: Human-readable project name
- `version`: Semantic version (X.X.X format)

### Database Section
- `provider`: Database provider (SqlServer, PostgreSQL, SQLite, MySQL)
- `connectionStringName`: Environment variable name for connection string
- `dataProjectPath`: Relative path to the data project (.csproj file)
- `dbContextName`: Name of the generated DbContext class
- `scaffoldOutputDirectory`: Subdirectory for generated entity models (relative to data project)

### SilphDatabase Section
- `enabled`: Whether to use Silph infrastructure database
- `connectionStringName`: Environment variable for Silph DB connection
- `registerProject`: Whether to register this project with Silph infrastructure

### Logging Section
- `structureEnabled`: Enable structured logging
- `bootstrapEnabled`: Enable bootstrap logging
- `maxLogRecords`: Maximum number of log records to retain

---

## Error Handling

All commands provide clear feedback:
- **Green (✓)**: Success messages
- **Yellow (⚠)**: Warnings (e.g., environment variable not set)
- **Red (✗)**: Errors with detailed messages

Common issues and solutions:

| Issue | Solution |
|-------|----------|
| "No silph.config.json found" | Run `silph init` first |
| "Environment variable not set" | Set the connection string variable or provide it manually when prompted |
| "dotnet-ef not found" | Install with `dotnet tool install --global dotnet-ef` |
| "Scaffolding failed" | Verify connection string, database accessibility, and provider package installation |

---

## Future Enhancements

Potential improvements:
- Non-interactive mode with command-line arguments
- Support for additional database providers
- Migration generation and application
- Template support for different project types
- Integration with project creation (dotnet new templates)
- Global tool packaging for easier installation

---

## Related Documentation

- **[Retrofitting Existing Projects](RETROFITTING_EXISTING_PROJECTS.md)** - Integrate Silph into existing codebases
- [Version Management](VERSION_MANAGEMENT.md) - Framework versioning commands
- [Configuration Schema](../Silph.Core/Configurations/README.md) - Detailed config file documentation
- [Framework Design](../README.md) - Overall Silph framework architecture
