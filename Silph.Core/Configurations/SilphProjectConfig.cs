namespace Silph.Core.Configurations
{
    public sealed class SilphProjectConfig
    {
        public SilphProjectInfo Project { get; init; } = new();
        public SilphDatabaseConfig Database { get; init; } = new();
        public SilphInfrastructureDatabaseConfig SilphDatabase { get; init; } = new();
        public SilphLoggingConfig Logging { get; init; } = new();
    }
}
