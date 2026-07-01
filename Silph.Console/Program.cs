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
                case "init":
                    HandleInitCommand(args.Skip(1).ToArray());
                    break;

                case "db":
                    HandleDbCommand(args.Skip(1).ToArray());
                    break;

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

    static void HandleInitCommand(string[] args)
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Silph Project Initialization");
        System.Console.WriteLine("============================");
        System.Console.ResetColor();
        System.Console.WriteLine();

        // Get project details
        System.Console.Write("Project Name: ");
        var projectName = System.Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(projectName))
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("✗ Project name is required.");
            System.Console.ResetColor();
            return;
        }

        System.Console.Write("Project ID (leave empty for auto-generated): ");
        var projectId = System.Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(projectId))
        {
            projectId = Guid.NewGuid().ToString("N").Substring(0, 8);
            System.Console.WriteLine($"  Generated ID: {projectId}");
        }

        // Optional: specify output directory
        var outputPath = args.Length > 0 && args[0] == "--output" && args.Length > 1
            ? args[1]
            : Directory.GetCurrentDirectory();

        var configPath = Path.Combine(outputPath, "silph.config.json");

        // Check if config already exists
        if (File.Exists(configPath))
        {
            System.Console.Write("Configuration file already exists. Overwrite? (y/N): ");
            var overwrite = System.Console.ReadLine()?.Trim().ToLower();
            if (overwrite != "y" && overwrite != "yes")
            {
                System.Console.WriteLine("Initialization cancelled.");
                return;
            }
        }

        // Create configuration
        var config = SilphConfigDefaults.Create(projectId, projectName);

        // Write configuration
        var writer = new SilphConfigWriter();
        var result = writer.WriteTemplate(configPath, config, overwrite: true);

        if (result.Success)
        {
            System.Console.WriteLine();
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("✓ Silph configuration created successfully!");
            System.Console.ResetColor();
            System.Console.WriteLine();
            System.Console.WriteLine($"  Location: {configPath}");
            System.Console.WriteLine($"  Project ID: {projectId}");
            System.Console.WriteLine($"  Project Name: {projectName}");
            System.Console.WriteLine();
            System.Console.WriteLine("Next steps:");
            System.Console.WriteLine("  1. Review and customize silph.config.json");
            System.Console.WriteLine("  2. Set up environment variables for connection strings");
            System.Console.WriteLine("  3. Run 'silph db setup' to configure database scaffolding");
            System.Console.WriteLine();
        }
        else
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"✗ Failed to create configuration: {string.Join(", ", result.Errors.Select(e => e.Template))}");
            System.Console.ResetColor();
        }
    }

    static void HandleDbCommand(string[] args)
    {
        if (args.Length == 0)
        {
            System.Console.WriteLine("Usage: silph db <setup|scaffold|info>");
            System.Console.WriteLine();
            System.Console.WriteLine("  setup     - Configure database settings and create directory structure");
            System.Console.WriteLine("  scaffold  - Run EF Core scaffolding (requires dotnet-ef)");
            System.Console.WriteLine("  info      - Display current database configuration");
            return;
        }

        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "setup":
                HandleDbSetup(args.Skip(1).ToArray());
                break;

            case "scaffold":
                HandleDbScaffold(args.Skip(1).ToArray());
                break;

            case "info":
                HandleDbInfo(args.Skip(1).ToArray());
                break;

            default:
                System.Console.WriteLine($"Unknown db subcommand: {subCommand}");
                System.Console.WriteLine("Use 'silph db' to see available subcommands.");
                break;
        }
    }

    static void HandleDbSetup(string[] args)
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Database Setup");
        System.Console.WriteLine("==============");
        System.Console.ResetColor();
        System.Console.WriteLine();

        // Find configuration file
        var configPath = FindConfigFile();
        if (configPath == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("✗ No silph.config.json found. Run 'silph init' first.");
            System.Console.ResetColor();
            return;
        }

        // Load configuration
        var reader = new SilphConfigReader();
        var configResult = reader.Read(configPath);
        if (configResult.Failed || configResult.Value == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"✗ Failed to read configuration: {string.Join(", ", configResult.Errors.Select(e => e.Template))}");
            System.Console.ResetColor();
            return;
        }

        var config = configResult.Value;
        var projectRoot = Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory();

        System.Console.WriteLine($"Project: {config.Project.Name} (ID: {config.Project.Id})");
        System.Console.WriteLine();

        // Ask for database provider if not set or confirm existing
        System.Console.WriteLine($"Current database provider: {config.Database.Provider}");
        System.Console.Write("Database provider (SqlServer/PostgreSQL/SQLite/MySQL) [press Enter to keep current]: ");
        var provider = System.Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(provider))
        {
            config = new SilphProjectConfig
            {
                Project = config.Project,
                Database = new SilphDatabaseConfig
                {
                    Provider = provider,
                    ConnectionStringName = config.Database.ConnectionStringName,
                    DataProjectPath = config.Database.DataProjectPath,
                    DbContextName = config.Database.DbContextName,
                    ScaffoldOutputDirectory = config.Database.ScaffoldOutputDirectory
                },
                SilphDatabase = config.SilphDatabase,
                Logging = config.Logging
            };
        }

        // Set up data project path
        System.Console.WriteLine();
        System.Console.WriteLine($"Current data project path: {config.Database.DataProjectPath}");
        System.Console.Write("Create data project directory? (Y/n): ");
        var createDataProject = System.Console.ReadLine()?.Trim().ToLower();
        if (createDataProject != "n" && createDataProject != "no")
        {
            var dataProjectPath = Path.Combine(projectRoot, $"{config.Project.Name}.Data");
            if (!Directory.Exists(dataProjectPath))
            {
                Directory.CreateDirectory(dataProjectPath);
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"✓ Created: {dataProjectPath}");
                System.Console.ResetColor();

                // Create Models subdirectory
                var modelsPath = Path.Combine(dataProjectPath, config.Database.ScaffoldOutputDirectory);
                Directory.CreateDirectory(modelsPath);
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"✓ Created: {modelsPath}");
                System.Console.ResetColor();
            }
            else
            {
                System.Console.WriteLine($"  Directory already exists: {dataProjectPath}");
            }
        }

        // Update DbContext name
        System.Console.WriteLine();
        System.Console.WriteLine($"Current DbContext name: {config.Database.DbContextName}");
        System.Console.Write("DbContext name [press Enter to keep current]: ");
        var dbContextName = System.Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(dbContextName))
        {
            config = new SilphProjectConfig
            {
                Project = config.Project,
                Database = new SilphDatabaseConfig
                {
                    Provider = config.Database.Provider,
                    ConnectionStringName = config.Database.ConnectionStringName,
                    DataProjectPath = config.Database.DataProjectPath,
                    DbContextName = dbContextName,
                    ScaffoldOutputDirectory = config.Database.ScaffoldOutputDirectory
                },
                SilphDatabase = config.SilphDatabase,
                Logging = config.Logging
            };
        }

        // Set up connection string name
        System.Console.WriteLine();
        System.Console.WriteLine($"Current connection string name: {config.Database.ConnectionStringName}");
        System.Console.Write("Connection string environment variable name [press Enter to keep current]: ");
        var connStringName = System.Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(connStringName))
        {
            config = new SilphProjectConfig
            {
                Project = config.Project,
                Database = new SilphDatabaseConfig
                {
                    Provider = config.Database.Provider,
                    ConnectionStringName = connStringName,
                    DataProjectPath = config.Database.DataProjectPath,
                    DbContextName = config.Database.DbContextName,
                    ScaffoldOutputDirectory = config.Database.ScaffoldOutputDirectory
                },
                SilphDatabase = config.SilphDatabase,
                Logging = config.Logging
            };
        }

        // Save updated configuration
        var writer = new SilphConfigWriter();
        var writeResult = writer.WriteTemplate(configPath, config, overwrite: true);

        if (writeResult.Success)
        {
            System.Console.WriteLine();
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("✓ Database setup completed!");
            System.Console.ResetColor();
            System.Console.WriteLine();
            System.Console.WriteLine("Configuration updated:");
            System.Console.WriteLine($"  Provider: {config.Database.Provider}");
            System.Console.WriteLine($"  DbContext: {config.Database.DbContextName}");
            System.Console.WriteLine($"  Connection String Variable: {config.Database.ConnectionStringName}");
            System.Console.WriteLine();
            System.Console.WriteLine("Next steps:");
            System.Console.WriteLine($"  1. Set environment variable: {config.Database.ConnectionStringName}");
            System.Console.WriteLine("  2. Install EF Core tools: dotnet tool install --global dotnet-ef");
            System.Console.WriteLine("  3. Run scaffolding: silph db scaffold");
            System.Console.WriteLine();
        }
        else
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"✗ Failed to save configuration: {string.Join(", ", writeResult.Errors.Select(e => e.Template))}");
            System.Console.ResetColor();
        }
    }

    static void HandleDbScaffold(string[] args)
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Database Scaffolding");
        System.Console.WriteLine("===================");
        System.Console.ResetColor();
        System.Console.WriteLine();

        // Find configuration file
        var configPath = FindConfigFile();
        if (configPath == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("✗ No silph.config.json found.");
            System.Console.ResetColor();
            return;
        }

        // Load configuration
        var reader = new SilphConfigReader();
        var configResult = reader.Read(configPath);
        if (configResult.Failed || configResult.Value == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"✗ Failed to read configuration: {string.Join(", ", configResult.Errors.Select(e => e.Template))}");
            System.Console.ResetColor();
            return;
        }

        var config = configResult.Value;
        var projectRoot = Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory();

        // Get connection string from environment
        var connectionString = Environment.GetEnvironmentVariable(config.Database.ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine($"⚠ Environment variable '{config.Database.ConnectionStringName}' not found.");
            System.Console.ResetColor();
            System.Console.Write("Enter connection string manually: ");
            connectionString = System.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("✗ Connection string is required.");
                System.Console.ResetColor();
                return;
            }
        }

        System.Console.WriteLine($"Provider: {config.Database.Provider}");
        System.Console.WriteLine($"DbContext: {config.Database.DbContextName}");
        System.Console.WriteLine($"Output: {config.Database.DataProjectPath}/{config.Database.ScaffoldOutputDirectory}");
        System.Console.WriteLine();

        // Build dotnet-ef command
        var dataProjectPath = Path.Combine(projectRoot, config.Database.DataProjectPath);
        var providerPackage = config.Database.Provider.ToLower() switch
        {
            "sqlserver" => "Microsoft.EntityFrameworkCore.SqlServer",
            "postgresql" => "Npgsql.EntityFrameworkCore.PostgreSQL",
            "sqlite" => "Microsoft.EntityFrameworkCore.Sqlite",
            "mysql" => "Pomelo.EntityFrameworkCore.MySql",
            _ => "Microsoft.EntityFrameworkCore.SqlServer"
        };

        var scaffoldCommand = $"dotnet ef dbcontext scaffold \"{connectionString}\" {providerPackage} " +
                              $"--project \"{dataProjectPath}\" " +
                              $"--output-dir {config.Database.ScaffoldOutputDirectory} " +
                              $"--context {config.Database.DbContextName} " +
                              $"--force";

        System.Console.WriteLine("Running scaffolding command...");
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine($"> {scaffoldCommand.Replace(connectionString, "***")}");
        System.Console.ResetColor();
        System.Console.WriteLine();

        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/c {scaffoldCommand}")
            {
                WorkingDirectory = projectRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(processInfo);
            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(output))
                    System.Console.WriteLine(output);

                if (process.ExitCode == 0)
                {
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("✓ Scaffolding completed successfully!");
                    System.Console.ResetColor();
                }
                else
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"✗ Scaffolding failed with exit code {process.ExitCode}");
                    if (!string.IsNullOrWhiteSpace(error))
                        System.Console.WriteLine(error);
                    System.Console.ResetColor();
                }
            }
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"✗ Error running scaffolding: {ex.Message}");
            System.Console.WriteLine();
            System.Console.WriteLine("Make sure dotnet-ef is installed:");
            System.Console.WriteLine("  dotnet tool install --global dotnet-ef");
            System.Console.ResetColor();
        }
    }

    static void HandleDbInfo(string[] args)
    {
        var configPath = FindConfigFile();
        if (configPath == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("✗ No silph.config.json found.");
            System.Console.ResetColor();
            return;
        }

        var reader = new SilphConfigReader();
        var configResult = reader.Read(configPath);
        if (configResult.Failed || configResult.Value == null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"✗ Failed to read configuration: {string.Join(", ", configResult.Errors.Select(e => e.Template))}");
            System.Console.ResetColor();
            return;
        }

        var config = configResult.Value;

        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Database Configuration");
        System.Console.WriteLine("=====================");
        System.Console.ResetColor();
        System.Console.WriteLine();
        System.Console.WriteLine($"  Provider:              {config.Database.Provider}");
        System.Console.WriteLine($"  Connection String Var: {config.Database.ConnectionStringName}");
        System.Console.WriteLine($"  Data Project Path:     {config.Database.DataProjectPath}");
        System.Console.WriteLine($"  DbContext Name:        {config.Database.DbContextName}");
        System.Console.WriteLine($"  Scaffold Output:       {config.Database.ScaffoldOutputDirectory}");
        System.Console.WriteLine();

        var connString = Environment.GetEnvironmentVariable(config.Database.ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connString))
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine($"  ⚠ Environment variable '{config.Database.ConnectionStringName}' not set");
            System.Console.ResetColor();
        }
        else
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine($"  ✓ Connection string configured");
            System.Console.ResetColor();
        }
        System.Console.WriteLine();
    }

    static string? FindConfigFile()
    {
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDir != null)
        {
            var configPath = Path.Combine(currentDir.FullName, "silph.config.json");
            if (File.Exists(configPath))
            {
                return configPath;
            }

            currentDir = currentDir.Parent;
        }

        return null;
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

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine("  Project Setup");
        System.Console.ResetColor();
        System.Console.WriteLine("  -------------");
        System.Console.WriteLine("  init [--output <path>]");
        System.Console.WriteLine("    Initialize a new project with Silph configuration");
        System.Console.WriteLine("    Creates silph.config.json with project settings");
        System.Console.WriteLine();
        System.Console.WriteLine("  db setup");
        System.Console.WriteLine("    Configure database settings and create directory structure");
        System.Console.WriteLine("    Interactive setup for data project and scaffolding");
        System.Console.WriteLine();
        System.Console.WriteLine("  db scaffold");
        System.Console.WriteLine("    Run EF Core database scaffolding");
        System.Console.WriteLine("    Generates DbContext and entity models from existing database");
        System.Console.WriteLine();
        System.Console.WriteLine("  db info");
        System.Console.WriteLine("    Display current database configuration");
        System.Console.WriteLine();

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine("  Version Management");
        System.Console.ResetColor();
        System.Console.WriteLine("  ------------------");
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

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine("  Help");
        System.Console.ResetColor();
        System.Console.WriteLine("  ----");
        System.Console.WriteLine("  help");
        System.Console.WriteLine("    Display this help information");
        System.Console.WriteLine();

        System.Console.WriteLine("EXAMPLES:");
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine("  # Initialize a new project");
        System.Console.ResetColor();
        System.Console.WriteLine("  silph init");
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine("  # Set up database configuration");
        System.Console.ResetColor();
        System.Console.WriteLine("  silph db setup");
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine("  # Run database scaffolding");
        System.Console.ResetColor();
        System.Console.WriteLine("  silph db scaffold");
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine("  # Version management");
        System.Console.ResetColor();
        System.Console.WriteLine("  silph version");
        System.Console.WriteLine("  silph version increment minor");
        System.Console.WriteLine("  silph version status beta");
        System.Console.WriteLine("  silph version release");
        System.Console.WriteLine("  silph version changelog \"Fixed bug in logging\" \"Added new feature\"");
        System.Console.WriteLine();
    }
}

