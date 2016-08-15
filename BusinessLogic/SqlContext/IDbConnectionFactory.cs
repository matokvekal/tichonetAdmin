using System.Data;

namespace Business_Logic.SqlContext
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
