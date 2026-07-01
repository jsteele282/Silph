# Silph Framework Design Specifications

## Overview

Silph is a personal toolkit framework designed as a reusable NuGet package foundation for various projects. It provides standardized tools and patterns for common development needs, with a focus on robust result handling, comprehensive messaging, and structured logging.

## Core Philosophy

Silph provides a **common tool kit for other projects to inherit**, offering standardized patterns while encouraging customization through inheritance. The framework is designed to handle the routine infrastructure concerns so that inheriting projects can focus on domain-specific logic.

---

## Documentation Index

### Getting Started
- **[Console Tools Guide](CONSOLE_TOOLS.md)** - Setup tools for new Silph-based projects
- **[Retrofitting Existing Projects](RETROFITTING_EXISTING_PROJECTS.md)** - Integrate Silph into existing codebases
- **[Quick Reference](QUICK_REFERENCE.md)** - New vs. existing project workflow comparison

### Framework Features
- **[Version Management](VERSION_MANAGEMENT.md)** - Version tracking and changelog tools
- **Configuration System** (see below) - Project and database configuration
- **Result Pattern** (see below) - Operation outcome handling
- **Message System** (see below) - Structured error and info messages

### Examples
- **[Pokedex Integration Example](../../../SILPH_INTEGRATION.md)** - Real-world retrofit example

---

## Silph.Core

The Core project represents fundamental functionalities for inheritance across projects.

### Result Pattern

The `Result` object carries the objective value of function calls, typically relating to:
- Business logic operations
- Database queries
- Frontend-to-backend communication
- Cross-layer data flow

**Key Features:**
- Collects messages along execution paths
- Provides built-in logging capabilities when available
- Informs receivers of messages and errors
- Generic `Result<T>` for value returns
- Non-generic `Result` for operation status

**Design Pattern:**
```
Result → Messages → Logging → Analysis
```

### Message System

Messages are structured with three primary components:

1. **Error Code**: Unique identifier for the message type
2. **Message Body**: Human-readable description
3. **Designation**: Message type classification

#### Six Message Levels

**End-User Specific (Non-Breaking):**
- **Info**: Informational messages for user awareness
- **Warning**: Cautionary messages without code impact

**Developer Specific:**
- **Debug**: Variable application by developers; never exposed in release builds

**Variable Accessibility (Primarily for Logs):**
- **Success**: Confirms successful operation completion
- **Error**: Indicates localized failure without structural impact
- **Critical**: Signals structural issues requiring immediate attention or program termination

### Message Repository Pattern

**Default Implementation:**
- `Messages` and `MessageCodes` classes provide baseline repository
- Available for direct use by inheritor projects

**Customization Encouraged:**
- Inheritor projects should create custom `Messages` and `MessageCodes` classes
- Inherit from Silph base classes
- Override and extend as necessary
- Tailor message repositories to project-specific needs

**Example Inheritance Pattern:**
```
Silph.Core.Messages (Base)
	↓
Pokemon.Core.Messages (Custom) → Adds Pokemon-specific codes
```

### Logging Architecture

Logging operates across **three distinct scopes**:

#### 1. None
- **Purpose**: Explicitly opt-out of logging
- **Use Case**: Performance-critical paths, sensitive operations
- **Behavior**: Message is not recorded

#### 2. Bootstrap
- **Purpose**: File-based logging for critical infrastructure
- **Target**: Log files on disk
- **Default Behavior**: Critical messages automatically use Bootstrap
- **Selective Usage**: Other message types can selectively push to Bootstrap
- **Rationale**: Captures important messages when database logging unavailable

#### 3. Structured
- **Purpose**: Database-driven analytics and monitoring
- **Target**: Silph-managed database
- **Components**: 
  - `LogRecords`: FIFO queue for recent logs
  - `LogStats`: Aggregated analytics
  - `Projects`: Project registration table
  - `Operations`: Function/operation tracking

---

## Structured Logging Deep Dive

### Project Registration

**Requirement:**
- Inheritor projects must establish configuration table
- Console command assigns Project ID and Name
- Registration creates connection to logging database

**Workflow:**
```
1. Create inheritor project
2. Run Silph console command to register project
3. Assign ProjectId and Name to Projects table
4. Connection established
5. Structured logging enabled
```

**Behavior:**
- **Connected**: Database collects Structured log calls
- **Not Connected**: Structured messages not logged or tracked

### LogRecords: Per-Project FIFO Queue

**Design:**
- Maintains last N logs per project
- First-In-First-Out queue structure
- Project-isolated capacity management

**Example:**
- Project A: Allowance for P logs
- Project B: Allowance for Q logs

**Behavior:**
- Push from Project A: Only P_i logs counted
- When P_(N+1) pushed: P_0 removed
- All Q_j logs (Project B) remain untouched

**Independence:**
Each project's log queue operates independently without cross-contamination.

### LogStats: Aggregated Analytics

**Purpose:**
- Maintain comprehensive log statistics
- Optimize storage by tracking metadata only

**Stored Data:**
- `MessageCode`: Unique message identifier
- `ProjectId`: Source project reference
- `MessageTypeKey`: Message level designation
- `OperationId`: Function/operation context
- `DateTime`: Timestamp of log creation

**Optimization:**
Full message bodies are NOT stored in LogStats; only structured metadata for analysis.

### Operations Tracking

**Ideal State:**
- Tracks the function/method where log was created
- Provides call-site context for debugging
- Enables operation-level analytics

**Implementation Considerations:**
- Stack frame inspection for automatic capture
- Manual operation context specification
- Async method handling (MoveNext scenarios)

---

## Silph.Data

Database abstraction layer providing foundational data access tools.

### Diagnostic & Health Utilities

**Capabilities:**
- Table existence verification
- Database connection status
- Schema validation
- Health check operations

### Connection Management

**Basic Operations:**
- Open connection
- Close connection
- Connection pooling support
- Transaction handling

### CRUD Operations

**Design:**
- Basic Create, Read, Update, Delete operations
- Wrapped in `Result` pattern for consistent error handling
- Returns structured responses with messages

### Query Executor

**Philosophy:**
> "The responsibility for the string's competency is on the caller."

**Behavior:**
- Accepts query string as input
- Handles connection logic
- Executes query
- Returns `Result` with outcome

**Responsibilities:**
- **Silph.Data**: Connection management, execution, result wrapping
- **Caller**: Query correctness, parameterization, SQL validity

**Future Consideration:**
Query parsing for diagnostic assistance (secondary priority).

---

## Silph.View

Window creation and management assistance layer.

### Planned Features

**Default Windows:**
- Standard application windows
- Debugging/diagnostic interfaces

**Common Structures:**
- Reusable control templates
- Layout patterns
- Style-sheet defaults

**Custom Form Tools:**
- Form builders
- Validation helpers
- Data binding utilities

### Design Considerations

**Framework Target:** TBD (WPF, WinForms, Avalonia, or framework-agnostic)

---

## Future Projects

### Silph.Image

**Purpose:**
Image management and manipulation toolkit.

**Planned Features:**
- Web scraping for image retrieval
- File allocation and organization
- File type management and conversion
- Format validation
- Image optimization

### Silph.Dashboard

**Purpose:**
Centralized monitoring and management interface.

**Planned Features:**
- Log tracking and visualization
- Image retrieval management
- File structure organization
- Message code browser
- Project registration UI
- Log analytics and reporting

### Additional Support Projects

Long-term roadmap includes various specialized toolkits to address common cross-project needs.

---

## Deployment Strategy

### Long-Term Goal

**NuGet Package:**
- Published as custom NuGet package(s)
- Versioned for compatibility tracking
- Consumed by personal projects
- Provides standardized foundation

**Benefits:**
- Centralized maintenance
- Consistent patterns across projects
- Simplified project bootstrapping
- Reusable infrastructure

### Development Approach

**Phase 1: Foundation**
- Core functionality implementation
- Internal testing and iteration

**Phase 2: Dogfooding**
- Use Silph in real projects (e.g., Pokédex)
- Validate API surface
- Identify pain points

**Phase 3: Refinement**
- Address discovered issues
- Optimize performance
- Complete documentation

**Phase 4: Publication**
- Package for NuGet
- Establish versioning strategy
- Publish for consumption

---

## Design Principles

1. **Inheritance Over Configuration**: Provide base implementations, encourage customization
2. **Explicit Over Implicit**: Clear opt-in behaviors (e.g., LogScope.None)
3. **Flexibility**: Support multiple patterns (file logging, database logging, no logging)
4. **Isolation**: Per-project resource management (log queues, configurations)
5. **Responsibility**: Clear contracts about what framework handles vs. caller responsibilities
6. **Extensibility**: Design for inheritance and override at every layer

---

## Version

**Document Version:** 1.0  
**Last Updated:** 2024  
**Status:** Living Document  

---

*This README reflects the current design vision and will evolve as Silph develops.*
