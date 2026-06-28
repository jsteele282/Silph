namespace Silph.Core.Configurations
{
    public static class SilphConfigDefaults
    {
        public static SilphProjectConfig Create(string projectId, string projectName)
        {
            return new SilphProjectConfig
            {
                Project = new SilphProjectInfo
                {
                    Id = projectId,
                    Name = projectName,
                    Version = "0.1.0"
                },
                Database = new SilphDatabaseConfig
                {
                    Provider = "SqlServer",
                    ConnectionStringName = $"{projectName.ToUpperInvariant()}_CONNECTION_STRING",
                    DataProjectPath = $"{projectName}.Data/{projectName}.Data.csproj",
                    DbContextName = $"{projectName}DbContext",
                    ScaffoldOutputDirectory = "Models"
                },
                SilphDatabase = new SilphInfrastructureDatabaseConfig
                {
                    Enabled = true,
                    ConnectionStringName = "SILPH_CONNECTION_STRING",
                    RegisterProject = true
                },
                Logging = new SilphLoggingConfig
                {
                    StructureEnabled = true,
                    BootstrapEnabled = true,
                    MaxLogRecords = 1000
                }
            };
        }
    }
}
