using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class ADOExtensions
    {
        public static string GetDatabaseName(this DbContext entities)
        {
            var connection = entities.Database.GetDbConnection();

            return new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
        }
        public static string GetServerName(this DbContext entities)
        {
            var connection = entities.Database.GetDbConnection();

            return new SqlConnectionStringBuilder(connection.ConnectionString)["Server"].ToString();
        }

        public static string GetDatabaseName(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }

        public static void TruncateAll(this DbContext entities)
        {
            SqlCommand cmd;
            var builder = new StringBuilder();
            var entityType = entities.GetType();
            var properties = entityType.GetProperties().Where(p => p.PropertyType.Name.StartsWith("ObjectSet"));
            var database = entities.GetDatabaseName();
            var entityConnection = entities.Database.GetDbConnection();
            var connection = new SqlConnection(entityConnection.ConnectionString);

            connection.Open();

            cmd = connection.CreateCommand();

            foreach (var property in properties)
            {
                DebugUtils.Break();

                //builder.AppendLineFormat("delete {0}.dbo.{1}", database, property.Name.Singularize());
            }

            cmd.CommandText = builder.ToString();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public static void Truncate(this DbContext entities, params string[] tables)
        {
            DbCommand cmd;
            var builder = new StringBuilder();
            var entityType = entities.GetType();
            var database = entities.GetDatabaseName();
            var connection = entities.Database.GetDbConnection();

            connection.Open();

            cmd = connection.CreateCommand();

            foreach (var table in tables)
            {
                builder.AppendLineFormat("delete {0}.dbo.{1}", database, table);
            }

            cmd.CommandText = builder.ToString();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public static T SaveIfNotExists<T>(this DbContext entities, Func<T, bool> query, Func<T> createFunc) where T : EntityObject
        {
            var entityType = entities.GetType();
            var objectSet = (IEnumerable<T>)entityType.GetProperties().Single(p => p.PropertyType.Name.StartsWith("ObjectSet") && p.PropertyType.GetGenericArguments().First() == typeof(T)).GetValue(entities, null);
            var entity = objectSet.SingleOrDefault(e => query(e));

            if (entity == null)
            {
                entity = createFunc();
                ((dynamic)objectSet).AddObject(entity);

                entities.SaveChanges();
            }

            return entity;
        }
    }
}
