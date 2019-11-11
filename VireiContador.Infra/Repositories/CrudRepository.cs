using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.FastCrud;
using Dapper.FastCrud.Configuration;
using VireiContador.Infra.Extensions;

namespace VireiContador.Infra.Repositories
{
    public abstract class CrudRepository<T> : BaseRepository where T : Infra.Models.Sql
    {
        private readonly FormattableString whereAtivo = $"Ativo = 1 ";
        private readonly FormattableString orderByDataCriacaoDesc = $"DataCriacao DESC";

        protected CrudRepository(string connectionString) : base(connectionString)
        {
        }

        protected T ExecuteGet(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Find<T>(statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout))
                    .Where($"Ativo = 1 AND Id = @Id")
                    .WithParameters(new { Id = id }))
                    .FirstOrDefault();
            }
        }

        protected IReadOnlyCollection<T> ExecuteList(FormattableString orderBy = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Find<T>(statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout))
                                 .OrderBy(orderBy ?? orderByDataCriacaoDesc)).ToList();
            }
        }

        protected IReadOnlyCollection<T> ExecuteListPage(int page, int quantity, FormattableString orderBy = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Find<T>(statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout))
                                 .OrderBy(orderBy ?? orderByDataCriacaoDesc)
                                 .Skip((page - 1) * quantity)
                                 .Top(quantity)).ToList();
            }
        }

        protected IReadOnlyCollection<T> ExecuteListFilter(FormattableString where, IDictionary<string, object> parameters, int page, int quantity, FormattableString orderBy = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                if (where.ArgumentCount == 0)
                {
                    where = $"1=1";
                }
                
                return connection.Find<T>(statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout))
                    .Where(where)
                    .OrderBy(orderBy ?? orderByDataCriacaoDesc)
                    .Skip((page - 1) * quantity)
                    .Top(quantity)
                    .WithParameters(parameters.ToDynamicObject())).ToList();
            }
        }

        protected T ExecuteInsert(T model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                connection.Insert<T>(model, statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout)));
                return model;
            }
        }

        protected void ExecuteUpdate(T model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                connection.Update<T>(model, statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout)));
            }
        }

        protected void ExecuteDelete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var model = Activator.CreateInstance(typeof(T)) as T;
                model.Id = id;
                connection.Delete<T>(model, statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout)));
            }
        }

        protected int ExecuteCount()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Count<T>(statement => statement.WithTimeout(TimeSpan.FromSeconds(Timeout)));
            }
        }
    }
}
