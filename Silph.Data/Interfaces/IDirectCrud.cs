using Silph.Core.Objects;

namespace Silph.Data.Interfaces
{
    public interface IDirectCrud
    {
        Result<T> Insert<T>(string tableName, Dictionary<string, object?> values);
        Result<T> Update<T>(string tableName, Dictionary<string, object?> values, string whereClause, object? paramaters = null);
        Result<T> Delete<T>(string tableName, string whereClause, object? paramaters = null);
    }
}
