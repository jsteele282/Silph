using Silph.Core.Configurations;

namespace Silph.Console;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "version":
                    HandleVersionCommand(args.Skip(1).ToArray());
                    break;

                case "help":
                case "--help":
                case "-h":
                    ShowHelp();
                    break;

                default:
                    System.Console.WriteLine($"Unknown command: {command}");
                    System.Console.WriteLine("Use 'silph help' for available commands.");
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Error: {ex.Message}");
            System.Console.ResetColor();
        }
    }

    static void HandleVersionCommand(string[] args)
    {
        var versionPath = GetVersionFilePath();

        if (args.Length == 0)
        {
            DisplayCurrentVersion(versionPath);
            return;
        }

        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "increment":
                HandleVersionIncrement(versionPath, args.Skip(1).ToArray());
                break;

            case "status":
                HandleVersionStatus(versionPath, args.Skip(1).ToArray());
                break;

            case "release":
                HandleVersionRelease(versionPath, args.Skip(1).ToArray());
                break;

            case "changelog":
                HandleVersionChangelog(versionPath, args.Skip(1).ToArray());
                break;

            default:
                System.Console.WriteLine($"Unknown version subcommand: {subCommand}");
                System.Console.WriteLine("Available: increment, status, release, changelog");
                break;
        }
    }

    static void HandleVersionIncrement(string versionPath, string[] args)
    {
        if (args.Length == 0)
        {
            System.Console.WriteLine("Usage: silph version increment <major|minor|patch>");
            return;
        }

        var version = SilphVersion.FromFile(versionPath);
        var incrementType = args[0].ToLower();

        SilphVersion newVersion = incrementType switch
        {
            "major" => version.IncrementMajor(),
            "minor" => version.IncrementMinor(),
            "patch" => version.IncrementPatch(),
            _ => throw new ArgumentException($"Invalid increment type: {incrementType}. Use major, minor, or patch.")
        };

        newVersion.SaveToFile(versionPath);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"✓ Version incremented: {version.Version} → {newVersion.Version}");
        System.Console.ResetColor();
        System.Console.WriteLine($"  Status: {newVersion.ReleaseStatus}");
        System.Console.WriteLine($"  Description: {newVersion.Description}");
    }

    static void HandleVersionStatus(string versionPath, string[] args)
    {
        if (args.Length == 0)
        {
            System.Console.WriteLine("Usage: silph version status <alpha|beta|rc|stable>");
            return;
        }

        var version = SilphVersion.FromFile(versionPath);
        var newStatus = args[0].ToLower();

        if (!new[] { "alpha", "beta", "rc", "stable" }.Contains(newStatus))
        {
            throw new ArgumentException($"Invalid status: {newStatus}. Use alpha, beta, rc, or stable.");
        }

        var updatedVersion = version.WithReleaseStatus(newStatus);
        updatedVersion.SaveToFile(versionPath);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"✓ Release status updated: {version.ReleaseStatus} → {newStatus}");
        System.Console.ResetColor();
        System.Console.WriteLine($"  Version: {updatedVersion.FullVersion}");
    }

    static void HandleVersionRelease(string versionPath, string[] args)
    {
        var version = SilphVersion.FromFile(versionPath);

        DateTime? releaseDate = null;
        if (args.Length > 0 && args[0] == "--date" && args.Length > 1)
        {
            if (DateTime.TryParse(args[1], out var parsedDate))
            {
                releaseDate = parsedDate;
            }
        }

        var updatedVersion = releaseDate.HasValue 
            ? version.MarkAsReleased(releaseDate.Value) 
            : version.MarkAsReleased();

        updatedVersion.SaveToFile(versionPath);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"✓ Version {updatedVersion.Version} marked as released");
        System.Console.ResetColor();
        System.Console.WriteLine($"  Release Date: {updatedVersion.ReleaseDate:yyyy-MM-dd HH:mm:ss} UTC");
        System.Console.WriteLine($"  Status: {updatedVersion.ReleaseStatus}");
    }

    static void HandleVersionChangelog(string versionPath, string[] args)
    {
        if (args.Length == 0)
        {
            System.Console.WriteLine("Usage: silph version changelog <change1> [change2] [change3] ...");
            return;
        }

        var version = SilphVersion.FromFile(versionPath);
        var updatedVersion = version.WithChangelog(args);
        updatedVersion.SaveToFile(versionPath);

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"✓ Changelog updated for version {version.Version}");
        System.Console.ResetColor();
        System.Console.WriteLine($"  Changes added: {args.Length}");
        foreach (var change in args)
        {
            System.Console.WriteLine($"    - {change}");
        }
    }

    static void DisplayCurrentVersion(string versionPath)
    {
        var version = SilphVersion.FromFile(versionPath);

        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Silph Framework Version Information");
        System.Console.WriteLine("====================================");
        System.Console.ResetColor();
        System.Console.WriteLine();
        System.Console.WriteLine($"  Version:        {version.Version}");
        System.Console.WriteLine($"  Full Version:   {version.FullVersion}");
        System.Console.WriteLine($"  Status:         {version.ReleaseStatus}");
        System.Console.WriteLine($"  Release Date:   {(version.ReleaseDate.HasValue ? version.ReleaseDate.Value.ToString("yyyy-MM-dd") : "Not released")}");
        if (version.BuildNumber.HasValue)
            System.Console.WriteLine($"  Build Number:   {version.BuildNumber}");
        System.Console.WriteLine($"  Description:    {version.Description}");
        System.Console.WriteLine();
    }

    static string GetVersionFilePath()
    {
        // Start from the assembly location and work up to find version.json
        var assemblyPath = AppContext.BaseDirectory;
        var currentDir = new DirectoryInfo(assemblyPath);

        // Navigate up from bin/Debug/net10.0 to project root, then to solution root
        while (currentDir != null)
        {
            var versionPath = Path.Combine(currentDir.FullName, "version.json");
            if (File.Exists(versionPath))
            {
                return versionPath;
            }

            // Also check parent directory (solution root from project folder)
            if (currentDir.Parent != null)
            {
                versionPath = Path.Combine(currentDir.Parent.FullName, "version.json");
                if (File.Exists(versionPath))
                {
                    return versionPath;
                }
            }

            currentDir = currentDir.Parent;
        }

        throw new FileNotFoundException("version.json not found. Please run from Silph solution directory.");
    }

    static void ShowHelp()
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Silph Framework CLI");
        System.Console.WriteLine("===================");
        System.Console.ResetColor();
        System.Console.WriteLine();
        System.Console.WriteLine("USAGE:");
        System.Console.WriteLine("  silph <command> [options]");
        System.Console.WriteLine();
        System.Console.WriteLine("COMMANDS:");
        System.Console.WriteLine();
        System.Console.WriteLine("  version");
        System.Console.WriteLine("    Display current version information");
        System.Console.WriteLine();
        System.Console.WriteLine("  version increment <major|minor|patch>");
        System.Console.WriteLine("    Increment version number");
        System.Console.WriteLine("      major - Breaking changes (0.1.0 → 1.0.0)");
        System.Console.WriteLine("      minor - New features (0.1.0 → 0.2.0)");
        System.Console.WriteLine("      patch - Bug fixes (0.1.0 → 0.1.1)");
        System.Console.WriteLine();
        System.Console.WriteLine("  version status <alpha|beta|rc|stable>");
        System.Console.WriteLine("    Update release status");
        System.Console.WriteLine();
        System.Console.WriteLine("  version release [--date YYYY-MM-DD]");
        System.Console.WriteLine("    Mark current version as released (uses current date if not specified)");
        System.Console.WriteLine();
        System.Console.WriteLine("  version changelog <change1> [change2] ...");
        System.Console.WriteLine("    Add changelog entries for current version");
        System.Console.WriteLine();
        System.Console.WriteLine("  help");
        System.Console.WriteLine("    Display this help information");
        System.Console.WriteLine();
        System.Console.WriteLine("EXAMPLES:");
        System.Console.WriteLine();
        System.Console.WriteLine("  silph version");
        System.Console.WriteLine("  silph version increment minor");
        System.Console.WriteLine("  silph version status beta");
        System.Console.WriteLine("  silph version release");
        System.Console.WriteLine("  silph version changelog \"Fixed bug in logging\" \"Added new feature\"");
        System.Console.WriteLine();
    }
}

