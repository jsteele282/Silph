namespace Silph.Core.Configurations
{
    public sealed class SilphDatabaseConfig
    {
        public string Provider { get; init; } = "SqlServer";
        public string ConnectionStringName { get; init; } = string.Empty;
        public string DataProjectPath { get; init; } = string.Empty;
        public string DbContextName {  get; init; } = string.Empty;
        public string ScaffoldOutputDirectory { get; init; } = "Models";
    }
}
