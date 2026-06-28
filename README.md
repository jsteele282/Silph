# Silph Framework

A personal .NET toolkit framework providing reusable infrastructure patterns and utilities for building robust applications.

## Overview

Silph is designed as a foundational NuGet package framework that provides standardized tools for:
- Result pattern-based error handling
- Comprehensive message management
- Structured and file-based logging
- Database abstraction utilities
- UI/View helpers
- Image management and manipulation

## Version

**Current Version:** 0.1.0-alpha

See [version.json](version.json) for detailed version information.

## Architecture

Silph is organized into modular projects:

### Silph.Core
Core functionality including:
- **Result Pattern**: `Result` and `Result<T>` for operation outcomes
- **Message System**: Structured messaging with six levels (Info, Warning, Debug, Success, Error, Critical)
- **Logging**: Three-tier logging (None, Bootstrap, Structured)
- **Configuration**: Project configuration management

### Silph.Data
Database abstraction layer providing:
- Basic CRUD operations
- Query execution utilities
- Database health/diagnostic tools
- Connection management

### Silph.View
Window and UI management (planned):
- Default window templates
- Common controls and layouts
- Style sheet defaults

### Silph.Console
Command-line utilities for:
- Project registration
- Database initialization
- Version management

## Key Features

### Result Pattern
Handle operation outcomes with rich context:

```csharp
public Result<User> GetUser(int id)
{
	if (!Exists(id))
		return Result<User>.Fail(Messages.UserNotFound);

	var user = FetchUser(id);
	return Result<User>.Ok(user, Messages.Success);
}
```

### Message Repository Pattern
Inherit and extend messaging:

```csharp
public class MyMessages : Messages
{
	public Message CustomError { get; protected set; }

	public MyMessages() : base()
	{
		CustomError = new Message(Codes.CustomError, "Custom error occurred", MessageType.Error);
	}
}
```

### Structured Logging
Track logs in database with per-project isolation:

```csharp
Log(result.Messages, LogScope.Structured);  // Database
Log(criticalError, LogScope.Bootstrap);      // File
```

## Getting Started

### Installation

*(Future NuGet package)*

```bash
dotnet add package Silph.Core
```

### Prerequisites

- .NET 10 or higher
- Visual Studio 2026 or later (or compatible IDE)

### Build

```bash
cd Silph
dotnet build
```

### Configuration

See [Design Specifications](Silph.Core/Documentation/README.md) for detailed setup.

## Documentation

- [Design Specifications](Silph.Core/Documentation/README.md) - Complete framework architecture
- [Version Management](Silph.Core/Documentation/VersionManagement.md) - Versioning guidelines

## Development Status

**Current Phase:** Foundation Development (0.1.x)

This is a pre-release framework under active development. APIs are subject to change.

### Roadmap

- [x] Core Result pattern implementation
- [x] Message system architecture
- [x] Configuration management
- [x] Version tracking system
- [ ] Structured logging database implementation
- [ ] Console tooling for project registration
- [ ] Concrete data provider implementation
- [ ] View layer foundation
- [ ] Test coverage
- [ ] NuGet packaging

See [Recommendations.md](Silph.Core/Documentation/Recommendations.md) for detailed development tracking *(internal)*.

## Philosophy

Silph follows these design principles:

1. **Inheritance Over Configuration** - Provide bases, encourage customization
2. **Explicit Over Implicit** - Clear opt-in behaviors
3. **Flexibility** - Support multiple patterns and approaches
4. **Isolation** - Per-project resource management
5. **Clear Contracts** - Explicit responsibilities between framework and consumer

## Usage in Projects

Silph is designed to be inherited and extended:

```csharp
// In YourProject.Core
public class YourMessages : Silph.Core.Repositories.Messages
{
	protected override MessageCodes Codes => YourMessageCodes.Default;

	public Message YourCustomMessage { get; protected set; }

	public YourMessages() : base()
	{
		YourCustomMessage = new Message(
			Codes.YourCode,
			"Your message text",
			MessageType.Success
		);
	}
}
```

## License

*(Add your license here - MIT recommended for personal projects)*

## Author

Personal framework by jsteele282

## Changelog

See [version.json](version.json) for version history.

---

**Note:** This is a personal toolkit framework. It is not affiliated with any official Pokémon or Nintendo products.
