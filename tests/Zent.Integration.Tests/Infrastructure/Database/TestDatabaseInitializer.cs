using Npgsql;

namespace Zent.Integration.Tests.Infrastructure.Database;

public static class TestDatabaseInitializer
{
    public static async Task EnsureDatabaseReadyAsync(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var database = builder.Database;

        builder.Database = "postgres";

        await using (var connection = new NpgsqlConnection(builder.ConnectionString))
        {
            await connection.OpenAsync();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = """
                                       SELECT 1 
                                       FROM pg_database 
                                       WHERE datname = @db
                                   """;
            checkCmd.Parameters.AddWithValue("db", database!);

            var exists = await checkCmd.ExecuteScalarAsync();

            if (exists is null)
            {
                var createCmd = connection.CreateCommand();
                createCmd.CommandText = $"CREATE DATABASE \"{database}\"";
                await createCmd.ExecuteNonQueryAsync();
            }
        }

        builder.Database = database;

        await using var appConnection = new NpgsqlConnection(builder.ConnectionString);
        await appConnection.OpenAsync();
    }
}