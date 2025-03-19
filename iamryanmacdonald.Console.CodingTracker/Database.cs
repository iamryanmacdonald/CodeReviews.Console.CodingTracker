using Dapper;
using Microsoft.Data.Sqlite;

namespace iamryanmacdonald.Console.CodingTracker;

internal class Database
{
    private readonly string _connectionString;

    internal Database(string connectionString)
    {
        _connectionString = connectionString;

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using (var tableCommand = connection.CreateCommand())
        {
            tableCommand.CommandText =
                """
                CREATE TABLE IF NOT EXISTS coding_sessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Duration INTEGER,
                    EndTime TEXT,
                    StartTime TEXT
                )
                """;
            tableCommand.ExecuteNonQuery();
        }

        connection.Close();
    }

    internal void DeleteCodingSession(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "DELETE FROM coding_sessions WHERE Id = @Id";
        connection.Execute(sql, new { Id = id });

        connection.Close();
    }

    internal CodingSession? GetCodingSession(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "SELECT * FROM coding_sessions WHERE Id = @Id";
        var codingSession = connection.QuerySingle<CodingSession>(sql, new { Id = id });

        connection.Close();

        return codingSession;
    }

    internal List<CodingSession> GetCodingSessions()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "SELECT * FROM coding_sessions ORDER BY StartTime ASC, EndTime ASC, Id ASC";
        var codingSessions = connection.Query<CodingSession>(sql).ToList();

        connection.Close();

        return codingSessions;
    }

    internal void InsertCodingSession(DateTime startTime, DateTime endTime, int duration)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql =
            "INSERT INTO coding_sessions (Duration, EndTime, StartTime) VALUES (@Duration, @EndTime, @StartTime)";
        connection.Execute(sql, new { Duration = duration, EndTime = endTime, StartTime = startTime });

        connection.Close();
    }

    internal void UpdateCodingSession(int id, DateTime startTime, DateTime endTime, int duration)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql =
            "UPDATE coding_sessions SET Duration = @Duration, EndTime = @EndTime, StartTime = @StartTime WHERE Id = @Id";
        connection.Execute(sql, new { Duration = duration, EndTime = endTime, Id = id, StartTime = startTime });

        connection.Close();
    }
}