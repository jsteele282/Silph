using System.Text.Json;
using System.Text.Json.Nodes;

namespace Silph.Core.Configurations
{
    /// <summary>
    /// Represents the Silph framework version information.
    /// </summary>
    public sealed class SilphVersion
    {
        /// <summary>
        /// Full version string in MAJOR.MINOR.PATCH format.
        /// </summary>
        public string Version { get; init; } = "0.0.0";

        /// <summary>
        /// Major version number (breaking changes).
        /// </summary>
        public int VersionMajor { get; init; }

        /// <summary>
        /// Minor version number (new features, backwards compatible).
        /// </summary>
        public int VersionMinor { get; init; }

        /// <summary>
        /// Patch version number (bug fixes, backwards compatible).
        /// </summary>
        public int VersionPatch { get; init; }

        /// <summary>
        /// Release status (alpha, beta, rc, stable).
        /// </summary>
        public string ReleaseStatus { get; init; } = "alpha";

        /// <summary>
        /// Release date of this version (null if unreleased).
        /// </summary>
        public DateTime? ReleaseDate { get; init; }

        /// <summary>
        /// Build number (optional, for CI/CD tracking).
        /// </summary>
        public int? BuildNumber { get; init; }

        /// <summary>
        /// Description of this version.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Changelog entries for all versions.
        /// </summary>
        public Dictionary<string, ChangelogEntry>? Changelog { get; init; }

        /// <summary>
        /// Versioning metadata.
        /// </summary>
        public VersioningMetadata? Versioning { get; init; }

        /// <summary>
        /// Gets the version as a System.Version object.
        /// </summary>
        public Version AsVersion => new(VersionMajor, VersionMinor, VersionPatch);

        /// <summary>
        /// Gets the full version string with release status.
        /// </summary>
        public string FullVersion => BuildNumber.HasValue
            ? $"{Version}-{ReleaseStatus}+{BuildNumber}"
            : $"{Version}-{ReleaseStatus}";

        /// <summary>
        /// Reads the version configuration from the specified JSON file.
        /// </summary>
        /// <param name="filePath">Path to version.json file.</param>
        /// <returns>SilphVersion instance with configuration data.</returns>
        public static SilphVersion FromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Version file not found: {filePath}");

            var json = File.ReadAllText(filePath);
            var version = JsonSerializer.Deserialize<SilphVersion>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return version ?? throw new InvalidOperationException("Failed to deserialize version configuration.");
        }

        /// <summary>
        /// Attempts to read the version configuration from the specified JSON file.
        /// </summary>
        /// <param name="filePath">Path to version.json file.</param>
        /// <param name="version">Output version if successful.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryFromFile(string filePath, out SilphVersion? version)
        {
            version = null;

            try
            {
                version = FromFile(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Increments the major version number.
        /// </summary>
        /// <returns>New SilphVersion with incremented major version.</returns>
        public SilphVersion IncrementMajor() => new()
        {
            VersionMajor = VersionMajor + 1,
            VersionMinor = 0,
            VersionPatch = 0,
            Version = $"{VersionMajor + 1}.0.0",
            ReleaseStatus = "alpha",
            Description = $"Major version {VersionMajor + 1} development",
            Changelog = Changelog,
            Versioning = Versioning
        };

        /// <summary>
        /// Increments the minor version number.
        /// </summary>
        /// <returns>New SilphVersion with incremented minor version.</returns>
        public SilphVersion IncrementMinor() => new()
        {
            VersionMajor = VersionMajor,
            VersionMinor = VersionMinor + 1,
            VersionPatch = 0,
            Version = $"{VersionMajor}.{VersionMinor + 1}.0",
            ReleaseStatus = "alpha",
            Description = $"Version {VersionMajor}.{VersionMinor + 1} development",
            Changelog = Changelog,
            Versioning = Versioning
        };

        /// <summary>
        /// Increments the patch version number.
        /// </summary>
        /// <returns>New SilphVersion with incremented patch version.</returns>
        public SilphVersion IncrementPatch() => new()
        {
            VersionMajor = VersionMajor,
            VersionMinor = VersionMinor,
            VersionPatch = VersionPatch + 1,
            Version = $"{VersionMajor}.{VersionMinor}.{VersionPatch + 1}",
            ReleaseStatus = ReleaseStatus,
            Description = $"Version {VersionMajor}.{VersionMinor}.{VersionPatch + 1}",
            Changelog = Changelog,
            Versioning = Versioning
        };

        /// <summary>
        /// Saves the current version to the specified JSON file.
        /// Preserves the changelog and versioning metadata.
        /// </summary>
        /// <param name="filePath">Path to version.json file.</param>
        public void SaveToFile(string filePath)
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
            });

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Updates the release status of the current version.
        /// </summary>
        /// <param name="status">New release status (alpha, beta, rc, stable).</param>
        /// <returns>New SilphVersion with updated status.</returns>
        public SilphVersion WithReleaseStatus(string status) => new()
        {
            Version = Version,
            VersionMajor = VersionMajor,
            VersionMinor = VersionMinor,
            VersionPatch = VersionPatch,
            ReleaseStatus = status,
            ReleaseDate = ReleaseDate,
            BuildNumber = BuildNumber,
            Description = Description,
            Changelog = Changelog,
            Versioning = Versioning
        };

        /// <summary>
        /// Marks the version as released with the current date/time.
        /// </summary>
        /// <returns>New SilphVersion with release date set.</returns>
        public SilphVersion MarkAsReleased() => MarkAsReleased(DateTime.UtcNow);

        /// <summary>
        /// Marks the version as released with the specified date.
        /// </summary>
        /// <param name="releaseDate">The release date.</param>
        /// <returns>New SilphVersion with release date set.</returns>
        public SilphVersion MarkAsReleased(DateTime releaseDate) => new()
        {
            Version = Version,
            VersionMajor = VersionMajor,
            VersionMinor = VersionMinor,
            VersionPatch = VersionPatch,
            ReleaseStatus = ReleaseStatus,
            ReleaseDate = releaseDate,
            BuildNumber = BuildNumber,
            Description = Description,
            Changelog = Changelog,
            Versioning = Versioning
        };

        /// <summary>
        /// Adds or updates a changelog entry for the current version.
        /// </summary>
        /// <param name="changes">List of changes for this version.</param>
        /// <returns>New SilphVersion with updated changelog.</returns>
        public SilphVersion WithChangelog(params string[] changes)
        {
            var changelog = Changelog ?? new Dictionary<string, ChangelogEntry>();
            var updatedChangelog = new Dictionary<string, ChangelogEntry>(changelog);

            updatedChangelog[Version] = new ChangelogEntry
            {
                Date = ReleaseDate,
                Status = ReleaseStatus,
                Changes = changes.ToList()
            };

            return new SilphVersion
            {
                Version = Version,
                VersionMajor = VersionMajor,
                VersionMinor = VersionMinor,
                VersionPatch = VersionPatch,
                ReleaseStatus = ReleaseStatus,
                ReleaseDate = ReleaseDate,
                BuildNumber = BuildNumber,
                Description = Description,
                Changelog = updatedChangelog,
                Versioning = Versioning
            };
        }

        public override string ToString() => FullVersion;
    }

    /// <summary>
    /// Represents a changelog entry for a specific version.
    /// </summary>
    public sealed class ChangelogEntry
    {
        public DateTime? Date { get; init; }
        public string Status { get; init; } = "alpha";
        public List<string> Changes { get; init; } = new();
    }

    /// <summary>
    /// Versioning metadata and rules.
    /// </summary>
    public sealed class VersioningMetadata
    {
        public string Scheme { get; init; } = "Semantic Versioning 2.0.0";
        public string Description { get; init; } = "MAJOR.MINOR.PATCH";
        public VersioningRules? Rules { get; init; }
        public PreReleaseInfo? PreRelease { get; init; }
    }

    /// <summary>
    /// Versioning rules.
    /// </summary>
    public sealed class VersioningRules
    {
        public string Major { get; init; } = string.Empty;
        public string Minor { get; init; } = string.Empty;
        public string Patch { get; init; } = string.Empty;
    }

    /// <summary>
    /// Pre-release versioning information.
    /// </summary>
    public sealed class PreReleaseInfo
    {
        public string Note { get; init; } = string.Empty;
    }
}
