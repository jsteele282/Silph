using System.Text.Json;

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
            Description = $"Major version {VersionMajor + 1} development"
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
            Description = $"Version {VersionMajor}.{VersionMinor + 1} development"
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
            Description = $"Version {VersionMajor}.{VersionMinor}.{VersionPatch + 1}"
        };

        public override string ToString() => FullVersion;
    }
}
