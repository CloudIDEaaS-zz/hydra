using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Data.Objects.DataClasses;

namespace Utils
{
    public static class ADOExtensions
    {
        public static string GetDatabaseName(this ObjectContext entities)
        {
            var connection = (EntityConnection) entities.Connection;

            return new SqlConnectionStringBuilder(connection.StoreConnection.ConnectionString).InitialCatalog;
        }

        public static string GetDatabaseName(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }

        public static void TruncateAll(this ObjectContext entities)
        {
            SqlCommand cmd;
            var builder = new StringBuilder();
            var entityType = entities.GetType();
            var properties = entityType.GetProperties().Where(p => p.PropertyType.Name.StartsWith("ObjectSet"));
            var database = entities.GetDatabaseName();
            var entityConnection = (EntityConnection)entities.Connection;
            var connection = new SqlConnection(entityConnection.StoreConnection.ConnectionString);

            connection.Open();

            cmd = connection.CreateCommand();

            foreach (var property in properties)
            {
                builder.AppendLineFormat("delete {0}.dbo.{1}", database, property.Name.Singularize());
            }

            cmd.CommandText = builder.ToString();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public static void Truncate(this ObjectContext entities, params string[] tables)
        {
            SqlCommand cmd;
            var builder = new StringBuilder();
            var entityType = entities.GetType();
            var database = entities.GetDatabaseName();
            var entityConnection = (EntityConnection)entities.Connection;
            var connection = new SqlConnection(entityConnection.StoreConnection.ConnectionString);

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

        public static T SaveIfNotExists<T>(this ObjectContext entities, Func<T, bool> query, Func<T> createFunc) where T : EntityObject
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
