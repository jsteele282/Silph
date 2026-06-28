namespace Silph.Core.Configurations
{
    public sealed class SilphLoggingConfig
    {
        public bool StructureEnabled { get; init; } = true;
        public bool BootstrapEnabled { get; init; } = true;
        public int MaxLogRecords { get; init; } = 1000;
    }
}
