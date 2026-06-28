namespace Silph.Core.Configurations
{
    public sealed class SilphInfrastructureDatabaseConfig
    {
        public bool Enabled { get; set; } = true;
        public string ConnectionStringName { get; init; } = string.Empty;
        public bool RegisterProject { get; set; } = true;
    }
}
