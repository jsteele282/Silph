using Silph.Core.Objects;
using System.Data;

namespace Silph.Data.Interfaces
{
    public interface ISqlExecutor
    {
        Result<T> Execute<T>(string query, params object[] parameters);
        Result<T> Scalar<T>(string query, params object[] parameters);
        Result<List<T>> Query<T>(string query, Func<IDataReader, T> mape, params object[] parameters);
    }
}
