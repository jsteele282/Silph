using Silph.Core.Objects;

namespace Silph.Data.Interfaces
{
    public interface IDatabaseInspector
    {
        Result<bool> CanConnect();
        Result<bool> DatabaseExists(string dbName);
        Result<bool> TableExists(string tableName);
        Result<bool> ColumnExists(string tableName, string columnName);
    }
}
