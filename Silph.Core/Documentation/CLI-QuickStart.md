# Silph CLI - Quick Start Guide

## Overview

The Silph CLI provides command-line utilities for managing the Silph framework, including version management, project registration, and more.

## Building the CLI

```bash
cd Silph/Silph.Console
dotnet build
```

## Running Commands

### From Project Directory

```bash
cd Silph/Silph.Console
dotnet run -- <command> [options]
```

### After Publishing (Future)

```bash
silph <command> [options]
```

---

## Version Management Commands

### Display Current Version

```bash
dotnet run -- version
```

**Output:**
```
Silph Framework Version Information
====================================

  Version:        0.1.1
  Full Version:   0.1.1-alpha
  Status:         alpha
  Release Date:   Not released
  Description:    Version 0.1.1
```

---

### Increment Version

#### Patch Version (Bug Fixes)
```bash
dotnet run -- version increment patch
```
Example: `0.1.0` → `0.1.1`

#### Minor Version (New Features)
```bash
dotnet run -- version increment minor
```
Example: `0.1.1` → `0.2.0`

#### Major Version (Breaking Changes)
```bash
dotnet run -- version increment major
```
Example: `0.2.0` → `1.0.0`

**Output:**
```
✓ Version incremented: 0.1.0 → 0.1.1
  Status: alpha
  Description: Version 0.1.1
```

---

### Update Release Status

```bash
dotnet run -- version status <alpha|beta|rc|stable>
```

**Examples:**
```bash
# Mark as beta
dotnet run -- version status beta

# Mark as release candidate
dotnet run -- version status rc

# Mark as stable release
dotnet run -- version status stable
```

**Output:**
```
✓ Release status updated: alpha → beta
  Version: 0.1.1-beta
```

---

### Mark as Released

#### Use Current Date/Time
```bash
dotnet run -- version release
```

#### Specify Release Date
```bash
dotnet run -- version release --date 2024-12-15
```

**Output:**
```
✓ Version 0.1.1 marked as released
  Release Date: 2024-12-15 14:30:00 UTC
  Status: beta
```

---

### Add Changelog Entries

```bash
dotnet run -- version changelog "Change 1" "Change 2" "Change 3"
```

**Example:**
```bash
dotnet run -- version changelog "Added version increment CLI" "Fixed version file path detection" "Improved error handling"
```

**Output:**
```
✓ Changelog updated for version 0.1.1
  Changes added: 3
	- Added version increment CLI
	- Fixed version file path detection
	- Improved error handling
```

The changelog entries are added to `version.json`:
```json
{
  "changelog": {
	"0.1.1": {
	  "date": null,
	  "status": "alpha",
	  "changes": [
		"Added version increment CLI",
		"Fixed version file path detection",
		"Improved error handling"
	  ]
	}
  }
}
```

---

## Complete Workflow Examples

### Bug Fix Release

```bash
# 1. Increment patch version
dotnet run -- version increment patch

# 2. Add changelog entries
dotnet run -- version changelog "Fixed configuration writer exception handling" "Updated XML documentation"

# 3. Mark as released
dotnet run -- version release

# 4. Verify
dotnet run -- version
```

### Feature Release

```bash
# 1. Increment minor version
dotnet run -- version increment minor

# 2. Update status to beta (when feature complete)
dotnet run -- version status beta

# 3. Add changelog entries
dotnet run -- version changelog "Implemented structured logging" "Added project registration API" "Created LogRecords FIFO queue"

# 4. Test thoroughly...

# 5. Move to release candidate
dotnet run -- version status rc

# 6. Final testing...

# 7. Mark as stable and release
dotnet run -- version status stable
dotnet run -- version release

# 8. Verify
dotnet run -- version
```

### Major Version Release

```bash
# 1. Increment major version
dotnet run -- version increment major

# 2. Add breaking changes to changelog
dotnet run -- version changelog "BREAKING: Changed IDirectCrud interface" "BREAKING: Renamed Message.Type to Message.MessageType" "Added new Result validation methods"

# 3. Follow beta → rc → stable progression
dotnet run -- version status beta
# ... testing ...
dotnet run -- version status rc
# ... final testing ...
dotnet run -- version status stable

# 4. Release
dotnet run -- version release
```

---

## Version File Structure

The `version.json` file is automatically updated by the CLI:

```json
{
  "version": "0.1.1",
  "versionMajor": 0,
  "versionMinor": 1,
  "versionPatch": 1,
  "releaseStatus": "alpha",
  "releaseDate": null,
  "buildNumber": null,
  "description": "Version 0.1.1",
  "changelog": {
	"0.1.1": {
	  "date": null,
	  "status": "alpha",
	  "changes": [
		"Added version increment CLI",
		"Fixed version file path detection"
	  ]
	},
	"0.1.0": {
	  "date": null,
	  "status": "alpha",
	  "changes": [
		"Initial framework structure",
		"Result pattern implementation"
	  ]
	}
  }
}
```

---

## Tips

### Always Add Changelog Entries
After incrementing a version, immediately add changelog entries:
```bash
dotnet run -- version increment patch
dotnet run -- version changelog "Description of what changed"
```

### Use Semantic Versioning
- **Patch** (0.1.0 → 0.1.1): Bug fixes only
- **Minor** (0.1.0 → 0.2.0): New features, backward compatible
- **Major** (0.1.0 → 1.0.0): Breaking changes

### Pre-Release Progression
Follow this path for stability:
```
alpha (development) → beta (feature complete) → rc (final testing) → stable (production)
```

### Version 0.x.x Warning
Versions before 1.0.0 are considered pre-release. Breaking changes are allowed between minor versions.

---

## Error Handling

If the CLI can't find `version.json`, it will search up the directory tree from the executable location.

**Common Errors:**
- `version.json not found`: Make sure you're running from within the Silph solution directory
- `Invalid increment type`: Use `major`, `minor`, or `patch`
- `Invalid status`: Use `alpha`, `beta`, `rc`, or `stable`

---

## Future Commands (Planned)

```bash
# Project registration
dotnet run -- project register --name "MyApp" --id 1001

# Database initialization
dotnet run -- db init --connectionString "..."
dotnet run -- db migrate --version 1.0

# Log viewing
dotnet run -- log view --project MyApp --last 100
```

---

**Last Updated:** 2024  
**CLI Version:** 0.1.1
