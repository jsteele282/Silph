using Silph.Core.Objects;
using Silph.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Silph.Core.Configurations
{
    public sealed class SilphConfigWriter
    {
        public Result WriteTemplate(string filePath, SilphProjectConfig config, bool overwrite = false)
        {
            if (File.Exists(filePath) && !overwrite) return Result.Fail(Messages.Default.FileAlreadyExists);

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);

                var json = JsonSerializer.Serialize(
                    config,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );

                File.WriteAllText(filePath, json);

                return Result.Ok(Messages.Default.ProjectConfigurationWritten);
            }
            catch
            {
                return Result.Fail(Messages.Default.FileWriteFailed);
            }
        }
    }
}
