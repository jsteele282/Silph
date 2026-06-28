# Silph Version Management

## Overview

Silph uses **Semantic Versioning 2.0.0** (SemVer) for version tracking.

Format: **MAJOR.MINOR.PATCH** (e.g., `1.2.3`)

## Version Components

- **MAJOR**: Increment for breaking changes (incompatible API changes)
- **MINOR**: Increment for new features (backwards compatible)
- **PATCH**: Increment for bug fixes (backwards compatible)

### Pre-Release Versions (0.x.x)

During initial development (version 0.x.x), the API is considered unstable. Breaking changes may occur between minor versions.

Example: `0.1.0` → `0.2.0` may include breaking changes.

---

## Version Configuration File

Location: `Silph/version.json`

### Structure

```json
{
  "version": "0.1.0",
  "versionMajor": 0,
  "versionMinor": 1,
  "versionPatch": 0,
  "releaseStatus": "alpha",
  "releaseDate": null,
  "buildNumber": null,
  "description": "Initial development version"
}
```

### Fields

| Field | Type | Description |
|-------|------|-------------|
| `version` | string | Full version string (MAJOR.MINOR.PATCH) |
| `versionMajor` | int | Major version number |
| `versionMinor` | int | Minor version number |
| `versionPatch` | int | Patch version number |
| `releaseStatus` | string | Release status (alpha, beta, rc, stable) |
| `releaseDate` | DateTime? | When this version was released (null if unreleased) |
| `buildNumber` | int? | Optional build number for CI/CD tracking |
| `description` | string | Description of this version |

---

## Release Status Values

- **alpha**: Early development, highly unstable
- **beta**: Feature complete but may have bugs
- **rc**: Release candidate, final testing
- **stable**: Production-ready release

---

## Usage in Code

### Reading Version Information

```csharp
using Silph.Core.Configurations;

// Read version from file
var versionPath = Path.Combine(solutionRoot, "Silph", "version.json");
var version = SilphVersion.FromFile(versionPath);

Console.WriteLine(version.Version);        // "0.1.0"
Console.WriteLine(version.FullVersion);    // "0.1.0-alpha"
Console.WriteLine(version.ReleaseStatus);  // "alpha"
```

### Safe Reading (TryFromFile)

```csharp
if (SilphVersion.TryFromFile(versionPath, out var version))
{
	Console.WriteLine($"Silph Framework v{version.FullVersion}");
}
else
{
	Console.WriteLine("Could not load version information");
}
```

### Version as System.Version

```csharp
var version = SilphVersion.FromFile(versionPath);
var sysVersion = version.AsVersion;  // System.Version(0, 1, 0)

// Use for assembly versioning
[assembly: AssemblyVersion("0.1.0")]
```

### Programmatic Version Increment

```csharp
var current = SilphVersion.FromFile(versionPath);

// Increment major (0.1.0 → 1.0.0)
var nextMajor = current.IncrementMajor();

// Increment minor (0.1.0 → 0.2.0)
var nextMinor = current.IncrementMinor();

// Increment patch (0.1.0 → 0.1.1)
var nextPatch = current.IncrementPatch();
```

---

## Updating Version

### Manual Update

1. Edit `Silph/version.json`
2. Update `version`, `versionMajor`, `versionMinor`, or `versionPatch`
3. Update `description`
4. Set `releaseDate` when releasing
5. Update changelog section

### Example: Bug Fix Release

```json
{
  "version": "0.1.1",
  "versionMajor": 0,
  "versionMinor": 1,
  "versionPatch": 1,
  "releaseStatus": "alpha",
  "releaseDate": "2024-01-15T00:00:00Z",
  "description": "Fixed configuration writer exception handling"
}
```

### Example: Feature Release

```json
{
  "version": "0.2.0",
  "versionMajor": 0,
  "versionMinor": 2,
  "versionPatch": 0,
  "releaseStatus": "beta",
  "releaseDate": null,
  "description": "Added structured logging support"
}
```

---

## Changelog Management

The `version.json` file includes a changelog section:

```json
{
  "changelog": {
	"0.2.0": {
	  "date": "2024-02-01T00:00:00Z",
	  "status": "beta",
	  "changes": [
		"Implemented structured logging database",
		"Added project registration console commands",
		"Created LogRecords FIFO queue",
		"Added LogStats aggregation"
	  ]
	},
	"0.1.1": {
	  "date": "2024-01-15T00:00:00Z",
	  "status": "alpha",
	  "changes": [
		"Fixed exception handling in SilphConfigWriter",
		"Added XML documentation to public APIs"
	  ]
	},
	"0.1.0": {
	  "date": null,
	  "status": "alpha",
	  "changes": [
		"Initial framework structure",
		"Result pattern implementation",
		"Message system design"
	  ]
	}
  }
}
```

### Adding Changelog Entries

When creating a new version:

1. Add new entry with version number as key
2. Include `date`, `status`, and `changes` array
3. List significant changes in bullet points
4. Keep entries in reverse chronological order (newest first)

---

## Version Milestones

### 0.x.x Series (Pre-Release)
- **0.1.x**: Initial development, core features
- **0.2.x**: Structured logging implementation
- **0.3.x**: Data layer implementation
- **0.4.x**: View layer foundation
- **0.9.x**: Feature complete, bug fixing

### 1.0.0 (Stable Release)
- API stabilized
- Breaking changes increment major version
- Ready for NuGet publication
- Comprehensive documentation
- Full test coverage

---

## NuGet Package Versioning

When publishing to NuGet, use the version from `version.json`:

### Package Version Format

**Pre-release:**
```
0.1.0-alpha
0.2.0-beta.1
0.9.0-rc.2
```

**Stable:**
```
1.0.0
1.1.0
2.0.0
```

### .csproj Configuration

```xml
<PropertyGroup>
  <Version>0.1.0-alpha</Version>
  <PackageVersion>0.1.0-alpha</PackageVersion>
  <AssemblyVersion>0.1.0</AssemblyVersion>
  <FileVersion>0.1.0</FileVersion>
</PropertyGroup>
```

### Automated Version Injection

Consider using MSBuild targets to read `version.json` and inject into assembly:

```xml
<Target Name="ReadVersionFromJson" BeforeTargets="BeforeBuild">
  <!-- Read version.json and set properties -->
</Target>
```

---

## Best Practices

### 1. Version Increment Guidelines

**Increment MAJOR when:**
- Breaking API changes
- Removing public members
- Changing method signatures
- Incompatible behavior changes

**Increment MINOR when:**
- Adding new features
- Adding public APIs (backwards compatible)
- Deprecating functionality (not removing)

**Increment PATCH when:**
- Bug fixes
- Performance improvements
- Internal refactoring
- Documentation updates

### 2. Release Status Progression

```
alpha → beta → rc (release candidate) → stable
```

- **alpha**: Internal development, expect frequent breaking changes
- **beta**: Feature complete, API may still change
- **rc**: Final testing, no new features, only critical fixes
- **stable**: Production-ready, semver rules strictly enforced

### 3. Date Format

Use ISO 8601 format for dates:
```json
"releaseDate": "2024-01-15T14:30:00Z"
```

### 4. Build Numbers

Optional build number for CI/CD tracking:
```json
"buildNumber": 42
```

Full version becomes: `0.1.0-alpha+42`

---

## Migration Strategy

### From 0.x to 1.0

Before releasing 1.0.0:

1. **API Freeze**: No more breaking changes
2. **Documentation**: Complete XML docs for all public APIs
3. **Testing**: Comprehensive test coverage (>80%)
4. **Dogfooding**: Used successfully in at least 2 projects
5. **Migration Guide**: Document changes from 0.9.x
6. **Release Notes**: Comprehensive changelog

### Breaking Changes After 1.0

When introducing breaking changes after 1.0:

1. Increment major version (1.x.x → 2.0.0)
2. Deprecate old APIs in prior minor version if possible
3. Provide migration guide
4. Update changelog with breaking change notices
5. Consider providing compatibility shims

---

## Tools & Automation (Future)

### Planned CLI Commands

```bash
# Display current version
silph version

# Increment version
silph version increment major  # 0.1.0 → 1.0.0
silph version increment minor  # 0.1.0 → 0.2.0
silph version increment patch  # 0.1.0 → 0.1.1

# Set release status
silph version status beta

# Mark as released
silph version release --date "2024-01-15"

# Add changelog entry
silph version changelog add "Fixed critical bug in logging"
```

### CI/CD Integration

Automate version management in build pipeline:

```yaml
# Example GitHub Actions
- name: Read Version
  run: |
	$version = (Get-Content Silph/version.json | ConvertFrom-Json).version
	echo "VERSION=$version" >> $env:GITHUB_ENV

- name: Build with Version
  run: dotnet build -p:Version=${{ env.VERSION }}
```

---

**Document Version:** 1.0  
**Last Updated:** 2024
