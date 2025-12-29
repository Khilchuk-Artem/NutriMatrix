using Npgsql; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DatabaseHelper
{

    public static void CreateDatabaseIfNotExists(string newDatabaseName, string connectionString = "Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres")
    {

        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();

        using var checkCmd = new NpgsqlCommand(
            @"SELECT 1
              FROM pg_catalog.pg_database
              WHERE datname = @dbName;", conn);
        checkCmd.Parameters.AddWithValue("dbName", newDatabaseName);

        var exists = checkCmd.ExecuteScalar();
        if (exists == null)
        {
            using var createCmd = new NpgsqlCommand(
                $@"CREATE DATABASE ""{newDatabaseName}"";", conn);
            createCmd.ExecuteNonQuery();
        }

        conn.Close();
    }
}
