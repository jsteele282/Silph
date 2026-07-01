# Retrofitting Silph into Existing Projects

## Overview

This guide explains how to integrate Silph framework tools into an **existing project** that already has its own structure, database, and codebase. The Silph console tools can be adapted to work with projects that don't start from scratch.

## Scenario: Existing Project Structure

Let's use a real-world example - the **Pokedex** project:

```
Repository/
├── Pokedex.Console/        # Existing console app
├── Pokedex.Core/           # Existing business logic
├── Pokedex.Data/           # Existing data layer with models
│   ├── Ability.cs
│   ├── Pokemon.cs
│   └── ...
├── Pokedex.Tests/          # Existing tests
└── Pokedex.Viewer/         # Existing UI
```

**Key Characteristics of Existing Projects:**
- ✓ Already has a data project with entity models
- ✓ May or may not have a DbContext
- ✓ Has its own namespace conventions
- ✓ Has existing directory structure
- ✓ May use different database provider or connection approach

---

## Approach 1: Manual Configuration (Recommended for Existing Projects)

For existing projects, **manually creating the config file** gives you more control over how Silph integrates.

### Step 1: Create Configuration File Manually

Navigate to your project's root directory and create `silph.config.json`:

```bash
cd D:\OneDrive\Documents\Programming\Repository
# Create the file in the repository root, not inside Pokedex.Core or Pokedex.Data
```

```json
{
  "project": {
	"id": "pokedex-001",
	"name": "Pokedex",
	"version": "1.0.0"
  },
  "database": {
	"provider": "SqlServer",
	"connectionStringName": "POKEDEX_CONNECTION_STRING",
	"dataProjectPath": "Pokedex.Data/Pokedex.Data.csproj",
	"dbContextName": "PokedexContext",
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
	"maxLogRecords": 5000
  }
}
```

### Step 2: Customize for Your Existing Structure

**Key adjustments for existing projects:**

1. **Project ID**: Use a meaningful ID that represents your project
   ```json
   "id": "pokedex-001"  // Not auto-generated
   ```

2. **Version**: Match your current project version
   ```json
   "version": "1.0.0"  // Your actual version
   ```

3. **Data Project Path**: Point to your **existing** data project
   ```json
   "dataProjectPath": "Pokedex.Data/Pokedex.Data.csproj"
   ```

4. **DbContext Name**: Use your **existing** DbContext name (if you have one)
   ```json
   "dbContextName": "PokedexContext"  // Existing name
   ```

5. **Scaffold Output**: Point to where your models **already exist**
   ```json
   "scaffoldOutputDirectory": "Models"  // Or "." if models are in root of data project
   ```

### Step 3: Verify Configuration

```bash
# From repository root
D:\OneDrive\Documents\Programming\Repository\Silph\Silph.Console\bin\Debug\net10.0\Silph.Console.exe db info
```

This will display your configuration and validate the environment variable.

---

## Approach 2: Using `silph init` with Customization

You can use `silph init` as a starting point, then customize the generated config:

### Step 1: Generate Base Configuration

```bash
cd D:\OneDrive\Documents\Programming\Repository
D:\Path\To\Silph.Console.exe init
```

When prompted:
- **Project Name**: Enter "Pokedex"
- **Project ID**: Enter "pokedex-001" (or leave empty for auto-generation)

### Step 2: Update Generated Config

Edit `silph.config.json` to match your existing structure:

```json
{
  "project": {
	"id": "pokedex-001",
	"name": "Pokedex",
	"version": "1.0.0"  // ← Update to match actual version
  },
  "database": {
	"provider": "SqlServer",  // ← Update if using different provider
	"connectionStringName": "POKEDEX_CONNECTION_STRING",  // ← Match existing env var
	"dataProjectPath": "Pokedex.Data/Pokedex.Data.csproj",  // ← Point to existing project
	"dbContextName": "PokedexContext",  // ← Use existing DbContext name
	"scaffoldOutputDirectory": "Models"  // ← Match existing model location
  },
  "silphDatabase": {
	"enabled": false,  // ← Disable if not using Silph infrastructure yet
	"connectionStringName": "SILPH_CONNECTION_STRING",
	"registerProject": false
  },
  "logging": {
	"structureEnabled": false,  // ← Enable gradually as you adopt Silph features
	"bootstrapEnabled": false,
	"maxLogRecords": 5000
  }
}
```

---

## When to Use `silph db setup`

The `silph db setup` command is designed for **new projects** that need directory creation. For existing projects:

### ❌ **Don't Use If:**
- You already have a data project with models
- Your directory structure is established
- You have existing entity classes

### ✅ **Do Use If:**
- You want to add a **new** data project for a different database
- You're creating a microservice within the existing solution
- You want Silph to create a **separate** data layer

### Example: Adding a New Microservice

If you wanted to add a "Pokedex.Analytics" data project alongside the existing "Pokedex.Data":

```bash
# Create a separate config for the analytics service
silph init  # Create pokedex-analytics.config.json
silph db setup  # This WOULD create Pokedex.Analytics.Data directory

# Update config to avoid conflicts:
{
  "project": {
	"id": "pokedex-analytics",
	"name": "PokedexAnalytics",
	"version": "0.1.0"
  },
  "database": {
	"provider": "PostgreSQL",
	"connectionStringName": "POKEDEX_ANALYTICS_CONNECTION_STRING",
	"dataProjectPath": "PokedexAnalytics.Data/PokedexAnalytics.Data.csproj",
	"dbContextName": "PokedexAnalyticsContext",
	"scaffoldOutputDirectory": "Models"
  }
}
```

---

## When to Use `silph db scaffold`

The scaffolding command **can be useful** for existing projects in specific scenarios:

### Scenario 1: Re-scaffolding After Database Changes

If your database schema has changed and you want to regenerate models:

```bash
# 1. Ensure config points to existing structure
D:\Path\To\Silph.Console.exe db info

# 2. Backup your existing models (scaffolding uses --force flag!)
cp -r Pokedex.Data/Models Pokedex.Data/Models.backup

# 3. Run scaffolding
D:\Path\To\Silph.Console.exe db scaffold

# 4. Review changes and merge with custom model modifications
```

⚠️ **Warning**: The `--force` flag **overwrites existing files**. Always backup first!

### Scenario 2: Scaffolding a Different Database

If you're adding support for a different database provider:

```bash
# Update config to point to new provider
# Then scaffold to generate provider-specific models
silph db scaffold
```

### Scenario 3: Initial Scaffolding for Code-First Projects

If your existing project is **code-first** and you want to scaffold from an existing database:

1. Update config to point to the database
2. Create a temporary output directory
3. Scaffold into that directory
4. Merge with your existing code-first entities

---

## Common Existing Project Scenarios

### Scenario: Project Without DbContext

If your existing project **doesn't have a DbContext** yet (manual data access, Dapper, etc.):

1. **Add EF Core packages** to your data project:
   ```bash
   cd Pokedex.Data
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Design
   ```

2. **Create Silph config** with your existing model structure:
   ```json
   {
	 "database": {
	   "provider": "SqlServer",
	   "connectionStringName": "POKEDEX_CONNECTION_STRING",
	   "dataProjectPath": "Pokedex.Data/Pokedex.Data.csproj",
	   "dbContextName": "PokedexContext",
	   "scaffoldOutputDirectory": "."  // Models already in root
	 }
   }
   ```

3. **Run scaffold** to generate just the DbContext:
   ```bash
   silph db scaffold
   ```

4. **Review and customize** the generated DbContext to work with your existing models

### Scenario: Multiple Database Support

If your project needs to support multiple databases:

Create **multiple config files**:

```
Repository/
├── silph.config.json              # Primary database (SQL Server)
├── silph.config.postgres.json     # PostgreSQL variant
└── silph.config.sqlite.json       # SQLite variant
```

Then modify the Silph commands to accept a `--config` parameter (future enhancement), or manually switch configs.

### Scenario: Different Namespace Conventions

If your project uses different namespace conventions:

**Current Limitation**: Silph assumes the namespace matches the project name.

**Workaround**:
1. Let Silph scaffold to a temporary directory
2. Copy generated files to your actual structure
3. Manually adjust namespaces to match your conventions

**Future Enhancement**: Add namespace configuration to `silph.config.json`.

---

## Integration Checklist for Existing Projects

- [ ] **Analyze existing structure**
  - [ ] Identify data project(s)
  - [ ] Locate existing DbContext (if any)
  - [ ] Note namespace conventions
  - [ ] Document current database provider and connection approach

- [ ] **Create or customize config**
  - [ ] Create `silph.config.json` in repository root
  - [ ] Set meaningful project ID and actual version
  - [ ] Point to existing data project path
  - [ ] Use existing DbContext name (if applicable)
  - [ ] Match existing model directory structure
  - [ ] Configure connection string environment variable

- [ ] **Validate configuration**
  - [ ] Run `silph db info` to verify settings
  - [ ] Check that environment variable is set correctly
  - [ ] Confirm paths point to actual project files

- [ ] **Adopt Silph features gradually**
  - [ ] Start with configuration and logging disabled
  - [ ] Test scaffolding in a non-production branch
  - [ ] Enable Silph infrastructure features incrementally
  - [ ] Update documentation as you integrate

- [ ] **Test integration**
  - [ ] Backup existing code before scaffolding
  - [ ] Run scaffold in test environment first
  - [ ] Verify generated code matches existing patterns
  - [ ] Merge changes carefully, preserving custom logic

---

## Best Practices for Existing Projects

### 1. **Start Minimal**
Don't try to adopt all Silph features at once. Start with just the configuration file:

```json
{
  "project": { /* ... */ },
  "database": { /* ... */ },
  "silphDatabase": { "enabled": false },  // ← Disable infrastructure
  "logging": { "structureEnabled": false }  // ← Disable advanced logging
}
```

### 2. **Use Version Control**
Always commit your working state before running Silph commands that modify files:

```bash
git add .
git commit -m "Before Silph integration"
silph db scaffold
git diff  # Review changes before committing
```

### 3. **Backup Before Scaffolding**
The `--force` flag overwrites files. Always backup:

```bash
cp -r Pokedex.Data/Models Pokedex.Data/Models.backup
```

### 4. **Test in a Branch**
Create a feature branch for Silph integration:

```bash
git checkout -b feature/silph-integration
# Make changes
# Test thoroughly
git checkout main
git merge feature/silph-integration
```

### 5. **Document Your Decisions**
Add a `SILPH_INTEGRATION.md` file to your project documenting:
- Why you're adopting Silph
- Which features you're using
- Custom configuration choices
- Deviations from Silph defaults

### 6. **Preserve Custom Code**
If you have custom model logic (validation, computed properties, etc.), be careful with scaffolding:

**Option A**: Use partial classes
```csharp
// Pokemon.cs (scaffolded, regenerated)
public class Pokemon { }

// Pokemon.Custom.cs (your code, never touched by scaffold)
public partial class Pokemon 
{
	public string DisplayName => $"{Name} (#{PokedexNumber})";
}
```

**Option B**: Never re-scaffold
- Scaffold once to get initial structure
- Maintain models manually going forward
- Update `silph.config.json` to document "no-scaffold" policy

---

## Example: Retrofitting Pokedex

Here's a complete example of retrofitting Silph into the existing Pokedex project:

### Step 1: Create Configuration

**File**: `D:\OneDrive\Documents\Programming\Repository\silph.config.json`

```json
{
  "project": {
	"id": "pokedex-001",
	"name": "Pokedex",
	"version": "1.2.0"
  },
  "database": {
	"provider": "SqlServer",
	"connectionStringName": "POKEDEX_CONNECTION_STRING",
	"dataProjectPath": "Pokedex.Data/Pokedex.Data.csproj",
	"dbContextName": "PokedexContext",
	"scaffoldOutputDirectory": "."
  },
  "silphDatabase": {
	"enabled": false,
	"connectionStringName": "SILPH_CONNECTION_STRING",
	"registerProject": false
  },
  "logging": {
	"structureEnabled": false,
	"bootstrapEnabled": true,
	"maxLogRecords": 10000
  }
}
```

### Step 2: Verify Environment

```powershell
# Check if connection string is set
$env:POKEDEX_CONNECTION_STRING
# If not set:
$env:POKEDEX_CONNECTION_STRING = "Server=localhost;Database=PokedexDB;Trusted_Connection=True;"
```

### Step 3: Validate Configuration

```bash
cd D:\OneDrive\Documents\Programming\Repository
D:\OneDrive\Documents\Programming\Repository\Silph\Silph.Console\bin\Debug\net10.0\Silph.Console.exe db info
```

Expected output:
```
Database Configuration
=====================

  Provider:              SqlServer
  Connection String Var: POKEDEX_CONNECTION_STRING
  Data Project Path:     Pokedex.Data/Pokedex.Data.csproj
  DbContext Name:        PokedexContext
  Scaffold Output:       .

  ✓ Connection string configured
```

### Step 4: Optional - Test Scaffolding (in branch)

```bash
git checkout -b test/silph-scaffold
cp -r Pokedex.Data Pokedex.Data.backup
D:\OneDrive\Documents\Programming\Repository\Silph\Silph.Console\bin\Debug\net10.0\Silph.Console.exe db scaffold
# Review changes, then decide whether to keep or discard
```

---

## Future Enhancements for Existing Projects

Planned improvements to make Silph more existing-project-friendly:

1. **Non-destructive scaffolding** - Option to generate to a separate directory
2. **Namespace configuration** - Specify custom namespaces in config
3. **Selective scaffolding** - Choose which tables to scaffold
4. **Merge mode** - Detect existing models and merge changes
5. **Config migration tool** - Convert existing appsettings.json to silph.config.json
6. **Dry-run mode** - Preview what scaffold will generate without writing files

---

## Summary

**For Existing Projects:**
- ✅ **Manual config creation** is often better than `silph init`
- ✅ **Customize heavily** to match your existing structure
- ⚠️ **Be cautious with `db setup`** - it creates new directories
- ⚠️ **Backup before `db scaffold`** - it uses --force flag
- ✅ **Adopt incrementally** - start with basic config, add features gradually
- ✅ **Test in branches** - don't run Silph commands directly on main

**The core value of Silph for existing projects** is not scaffolding, but providing a **standardized configuration format** and **common infrastructure** that multiple projects can share and build upon.
