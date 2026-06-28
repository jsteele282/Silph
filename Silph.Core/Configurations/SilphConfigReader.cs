using Silph.Core.Objects;
using Silph.Core.Repositories;
using System.Text.Json;

namespace Silph.Core.Configurations
{
    public sealed class SilphConfigReader
    {
        public Result<SilphProjectConfig?> Read(string filePath)
        {
            Result<SilphProjectConfig?> result = new();

            if (string.IsNullOrWhiteSpace(filePath)) return Result<SilphProjectConfig?>.Fail(Messages.Default.RequiredValueMissing);
            if (!File.Exists(filePath)) return Result<SilphProjectConfig?>.Fail(Messages.Default.FileNotFound);

            try
            {
                var json = File.ReadAllText(filePath);
                SilphProjectConfig? config = JsonSerializer.Deserialize<SilphProjectConfig>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

                if (config == null) result.AddError(Messages.Default.ConfigurationInvalid);
                else result.AddValue(config, message: Messages.Default.ProjectConfigurationRead);
            }
            catch
            {
                result.AddError(Messages.Default.ConfigurationInvalid);
            }

            return result;
        }
    }
}
