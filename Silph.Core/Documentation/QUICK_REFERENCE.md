# Silph Tools: New vs. Existing Projects - Quick Reference

## When to Use Each Command

| Command | New Project | Existing Project | Notes |
|---------|-------------|------------------|-------|
| `silph init` | ✅ **Recommended** | ⚠️ **With Caution** | For existing: manual config often better |
| `silph db setup` | ✅ **Recommended** | ❌ **Avoid** | Creates new directories; conflicts with existing structure |
| `silph db scaffold` | ✅ **Recommended** | ⚠️ **Backup First** | Uses `--force` flag; overwrites existing files |
| `silph db info` | ✅ **Yes** | ✅ **Yes** | Safe read-only operation |
| `silph version` | ✅ **Yes** | ✅ **Yes** | Safe read-only operation |

---

## Workflow Comparison

### New Project from Scratch

```bash
# Step 1: Initialize
cd MyNewProject
silph init
# Enter: "MyApp" and optionally project ID

# Step 2: Set up database
silph db setup
# Choose provider, confirm directory creation

# Step 3: Configure environment
$env:MYAPP_CONNECTION_STRING = "Server=..."

# Step 4: Scaffold database
silph db scaffold

# ✅ Complete project structure created
```

**Result:**
```
MyNewProject/
├── silph.config.json
├── MyApp.Data/
│   ├── MyApp.Data.csproj
│   ├── Models/
│   │   ├── MyAppDbContext.cs
│   │   ├── User.cs
│   │   └── ...
```

---

### Existing Project (e.g., Pokedex)

```bash
# Step 1: Create config manually
cd D:\Repository
# Create silph.config.json with existing paths

# Step 2: Validate
silph db info

# Step 3: Skip db setup (directories exist!)
# ❌ Don't run: silph db setup

# Step 4: Test scaffolding (optional, in branch)
git checkout -b test/scaffold
cp -r Pokedex.Data Pokedex.Data.backup
silph db scaffold
# Review, then decide to keep or discard

# ✅ Configuration layer added, no structure changes
```

**Result:**
```
Repository/
├── silph.config.json          # ← New config file
├── Pokedex.Console/           # ← Unchanged
├── Pokedex.Core/              # ← Unchanged
├── Pokedex.Data/              # ← Unchanged (unless scaffolded)
│   ├── Ability.cs
│   ├── Pokemon.cs
│   └── ...
```

---

## Configuration Approach

### New Project

**Use**: `silph init` interactive command

```bash
silph init
# Project Name: MyApp
# Project ID: (leave empty for auto)
```

**Generated Config:**
```json
{
  "project": {
	"id": "a3f8b9c2",           // Auto-generated
	"name": "MyApp",
	"version": "0.1.0"           // Default starter version
  },
  "database": {
	"provider": "SqlServer",    // Default provider
	"connectionStringName": "MYAPP_CONNECTION_STRING",
	"dataProjectPath": "MyApp.Data/MyApp.Data.csproj",
	"dbContextName": "MyAppDbContext",
	"scaffoldOutputDirectory": "Models"
  },
  "silphDatabase": {
	"enabled": true,             // Infrastructure enabled by default
	"connectionStringName": "SILPH_CONNECTION_STRING",
	"registerProject": true
  },
  "logging": {
	"structureEnabled": true,    // Full logging enabled
	"bootstrapEnabled": true,
	"maxLogRecords": 1000
  }
}
```

### Existing Project

**Use**: Manual creation with custom values

```json
{
  "project": {
	"id": "pokedex-001",         // Meaningful ID
	"name": "Pokedex",
	"version": "1.0.0"            // Actual current version
  },
  "database": {
	"provider": "SqlServer",     // Match existing or intended provider
	"connectionStringName": "POKEDEX_CONNECTION_STRING",  // Existing env var
	"dataProjectPath": "Pokedex.Data/Pokedex.Data.csproj",  // Actual path
	"dbContextName": "PokedexContext",  // Existing or desired name
	"scaffoldOutputDirectory": "."      // Models in root (not subfolder)
  },
  "silphDatabase": {
	"enabled": false,            // Start disabled
	"connectionStringName": "SILPH_CONNECTION_STRING",
	"registerProject": false
  },
  "logging": {
	"structureEnabled": false,   // Enable gradually
	"bootstrapEnabled": true,    // Safe to enable immediately
	"maxLogRecords": 10000       // Higher for production app
  }
}
```

---

## Risk Assessment

### `silph init` Risk

| Risk | New Project | Existing Project |
|------|-------------|------------------|
| **Overwrites existing config** | 🟢 Low (no config exists) | 🔴 High (may overwrite custom config) |
| **Wrong defaults** | 🟢 Low (defaults are sensible) | 🟡 Medium (may not match conventions) |
| **Naming conflicts** | 🟢 Low (clean slate) | 🟡 Medium (may not match namespaces) |

**Mitigation for Existing**: Create config manually or use `init` as template then customize.

---

### `silph db setup` Risk

| Risk | New Project | Existing Project |
|------|-------------|------------------|
| **Creates unwanted directories** | 🟢 Low (directories needed) | 🔴 High (conflicts with existing) |
| **Wrong paths** | 🟢 Low (follows conventions) | 🔴 High (doesn't detect existing) |
| **Overwrites config** | 🟡 Medium (updates database section) | 🔴 High (may break existing settings) |

**Mitigation for Existing**: Skip this command entirely; manually edit config instead.

---

### `silph db scaffold` Risk

| Risk | New Project | Existing Project |
|------|-------------|------------------|
| **Overwrites models** | 🟢 Low (no models exist) | 🔴 **CRITICAL** (destroys custom code) |
| **Wrong namespaces** | 🟢 Low (generates fresh) | 🔴 High (may not match conventions) |
| **Loses customizations** | 🟢 Low (no customizations) | 🔴 **CRITICAL** (validation, partials lost) |

**Mitigation for Existing**: 
1. **Always backup first**: `cp -r Project.Data Project.Data.backup`
2. **Test in branch**: `git checkout -b test/scaffold`
3. **Review diffs carefully**: `git diff`
4. **Consider not scaffolding**: Use config for settings only

---

## Best Practices Summary

### ✅ New Projects

| Practice | Reason |
|----------|--------|
| Use `silph init` | Fast, correct defaults |
| Use `silph db setup` | Creates proper structure |
| Follow all prompts | Guidance for new developers |
| Use default names | Consistency across projects |
| Enable all Silph features | Get full framework benefits |

### ✅ Existing Projects

| Practice | Reason |
|----------|--------|
| Create config manually | Precise control over settings |
| Skip `db setup` | Avoid directory conflicts |
| Backup before scaffolding | Protect custom code |
| Test in branches | Validate before committing |
| Enable features gradually | Reduce risk |
| Document integration | Team awareness |

---

## Decision Tree

```
Are you starting a brand new project?
├─ Yes → Use silph init + silph db setup
│         Follow all defaults
│         Enable all features
│
└─ No (Existing project)
   │
   ├─ Do you have existing data models?
   │  ├─ Yes → Create config MANUALLY
   │  │         Skip db setup
   │  │         Consider NOT scaffolding
   │  │
   │  └─ No → Safe to use silph init
   │           Safe to use silph db setup
   │
   ├─ Do you have a DbContext?
   │  ├─ Yes → Use existing name in config
   │  │         Be very careful with scaffold
   │  │
   │  └─ No → Can scaffold to generate one
   │           Use temporary directory first
   │
   └─ Do you have custom model code?
	  ├─ Yes → ⚠️ DO NOT SCAFFOLD ⚠️
	  │         Or use partial classes
	  │
	  └─ No → Scaffolding is safer
			  Still backup first
```

---

## Command Safety Ratings

### Read-Only Commands (Always Safe)
- ✅ `silph db info` - Display configuration
- ✅ `silph version` - Display version
- ✅ `silph help` - Display help

### Write Commands (Safe for New Projects)
- ✅ `silph init` - Creates config file (if none exists)
- ✅ `silph db setup` - Creates directories (if none exist)
- ⚠️ `silph db scaffold` - Uses `--force`, always backup

### Write Commands (Risky for Existing Projects)
- ⚠️ `silph init` - May not match existing conventions
- ⚠️ `silph db setup` - May conflict with existing structure
- 🔴 `silph db scaffold` - **WILL OVERWRITE** existing files

---

## Quick Reference: Files Created

### New Project (`silph init` + `silph db setup`)

```
ProjectRoot/
├── silph.config.json                    # Configuration
└── MyApp.Data/                          # Created by db setup
	├── MyApp.Data.csproj                # Created by db setup
	└── Models/                          # Created by db setup
		├── MyAppDbContext.cs            # Created by scaffold
		├── User.cs                      # Created by scaffold
		└── ...
```

### Existing Project (Manual Integration)

```
ProjectRoot/
├── silph.config.json                    # Created manually
├── ExistingProject.Data/                # Already existed
│   ├── ExistingProject.Data.csproj      # Already existed
│   ├── User.cs                          # Already existed
│   ├── Order.cs                         # Already existed
│   └── ...                              # Already existed
```

**Key Difference**: Only the config file is new; everything else untouched.

---

## Summary Table

| Aspect | New Project | Existing Project |
|--------|-------------|------------------|
| **Config Creation** | Interactive (`silph init`) | Manual (precise control) |
| **Directory Creation** | Automated (`silph db setup`) | Manual (avoid conflicts) |
| **Scaffolding** | Recommended | Risky (backup required) |
| **Feature Enablement** | All features | Gradual adoption |
| **Risk Level** | 🟢 Low | 🟡 Medium to 🔴 High |
| **Testing Strategy** | Direct on main | Branch + backup |
| **Documentation** | Optional | Essential |

---

**Related Documentation:**
- [Console Tools Guide](Silph/Silph.Core/Documentation/CONSOLE_TOOLS.md)
- [Retrofitting Existing Projects](Silph/Silph.Core/Documentation/RETROFITTING_EXISTING_PROJECTS.md)
- [Pokedex Integration Example](SILPH_INTEGRATION.md)
