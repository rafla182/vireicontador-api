using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace VireiContador.Infra.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string connectionString;
        protected const int Timeout = 1800;

        protected BaseRepository(string connectionString)
        {
            this.connectionString = connectionString;

            SqlMapper.AddTypeMap(typeof(string), DbType.AnsiString);
        }

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        protected T ExecuteQuery<T>(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(sql, parameters, commandTimeout: Timeout);
            }
        }

        protected IReadOnlyList<T> ExecuteQueryList<T>(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Query<T>(sql, parameters, commandTimeout: Timeout).ToList();
            }
        }

        protected T ExecuteStoreProcedure<T>(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(sql, parameters, commandType: CommandType.StoredProcedure, commandTimeout: Timeout);
            }
        }

        protected dynamic ExecuteStoreProcedure(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault(sql, parameters, commandType: CommandType.StoredProcedure, commandTimeout: Timeout);
            }
        }

        protected dynamic ExecuteQuery(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault(sql, parameters, commandTimeout: Timeout);
            }
        }

        protected IReadOnlyList<T> ExecuteStoreProcedureList<T>(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure, commandTimeout: Timeout).ToList();
            }
        }

        protected int Execute(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Execute(sql, parameters, commandTimeout: Timeout);
            }
        }

        protected T1 ExecuteQueryMap<T1, T2>(string sql, Func<T1, T2, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.FirstOrDefault();
            }
        }

        protected T1 ExecuteQueryMap<T1, T2, T3>(string sql, Func<T1, T2, T3, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.FirstOrDefault();
            }
        }

        protected T1 ExecuteQueryMap<T1, T2, T3, T4>(string sql, Func<T1, T2, T3, T4, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.FirstOrDefault();
            }
        }

        protected IReadOnlyList<T1> ExecuteQueryMapList<T1, T2>(string sql, Func<T1, T2, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.ToList();
            }
        }

        protected IReadOnlyList<T1> ExecuteQueryMapList<T1, T2, T3>(string sql, Func<T1, T2, T3, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.ToList();
            }
        }

        protected IReadOnlyList<T1> ExecuteQueryMapList<T1, T2, T3, T4>(string sql, Func<T1, T2, T3, T4, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.ToList();
            }
        }

        protected IReadOnlyList<T1> ExecuteQueryMapList<T1, T2, T3, T4, T5>(string sql, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.ToList();
            }
        }

        protected IReadOnlyList<T1> ExecuteQueryMapList<T1, T2, T3, T4, T5, T6>(string sql, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.ToList();
            }
        }

        protected IReadOnlyList<T1> ExecuteQueryMapList<T1, T2, T3, T4, T5, T6, T7>(string sql, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var t1 = connection.Query(sql, map, splitOn: splitOn, param: parameters);
                return t1.ToList();
            }
        }
        
        protected bool ExecuteAny(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<int>(sql, parameters, commandTimeout: Timeout) > 0;
            }
        }
    }
}