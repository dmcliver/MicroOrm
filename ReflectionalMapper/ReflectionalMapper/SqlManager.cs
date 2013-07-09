using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace ReflectionalMapper
{
    public class SqlManager
    {
        private readonly ReflectionalMapper _mapper;
        private readonly SqlBuilder _builder;
        private readonly string _connectionName;

        public SqlManager(string connectionName):this(connectionName, new ReflectionalMapper(), new SqlBuilder()){}

        public SqlManager(string connectionName, ReflectionalMapper mapper, SqlBuilder builder)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (builder == null) throw new ArgumentNullException("builder");

            _connectionName = connectionName;
            _mapper = mapper;
            _builder = builder;
        }

        public IEnumerable<T> ExecQuery<T>(string sqlCmd, Dictionary<string,object> sqlParams = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            IEnumerable<T> entities;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(sqlCmd, connection);

                if (sqlParams != null)
                    AddParams(sqlCommand, sqlParams);
                
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                entities = _mapper.Map<T>(sqlDataReader);
            }
            return entities;
        }

        private void AddParams(SqlCommand sqlCommand, Dictionary<string, object> sqlParams)
        {
            foreach (KeyValuePair<string, object> pair in sqlParams)
            {
                sqlCommand.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
            }
        }

        public IEnumerable<T> FindBy<T>(Expression<Func<T, object>> field, object val)
        {
            string sql = _builder.BuildFromExpression(field, val);
            return ExecQuery<T>(sql, new Dictionary<string, object>{{_builder.ColumnName, val}});
        }

        public IEnumerable<T> FindAll<T>()
        {
            return ExecQuery<T>("SELECT * FROM " + typeof (T).Name);
        }
 
        public void Save<T>(T entity, Expression<Func<T,object>> autoIncrementIdToExclude = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            string sqlCmd = _builder.BuildInsertStatement(entity, autoIncrementIdToExclude);
            ExecuteNonQuery(connectionString, sqlCmd);
        }

        public void Update<T>(T entity, params Expression<Func<T, object>>[] ids)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            string sqlCmd = _builder.BuildUpdateStatement(entity, ids);
            ExecuteNonQuery(connectionString, sqlCmd);
        }

        public void Delete<T>(T entity, params Expression<Func<T, object>>[] ids)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            string sqlCmd = _builder.BuildDeleteStatement(entity, ids);
            ExecuteNonQuery(connectionString, sqlCmd);
        }

        private static void ExecuteNonQuery(string connectionString, string sqlCmd)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(sqlCmd, connection);
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
