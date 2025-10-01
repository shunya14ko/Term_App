using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace TermApp.Services;

public class MySqlService
{
    private readonly string? _connectionString;
    private static bool _schemaInitialized;
    private static readonly SemaphoreSlim SchemaLock = new(1, 1);

    public MySqlService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TermAppDb");
    }

    private async Task<MySqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new DataAccessException(
                "データベース接続文字列が設定されていません。appsettings.json で ConnectionStrings:TermAppDb を設定してください。",
                isConfigurationError: true);
        }

        var connection = new MySqlConnection(_connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await EnsureSchemaAsync(connection, cancellationToken).ConfigureAwait(false);
        }
        catch (MySqlException ex)
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            throw new DataAccessException("データベースに接続できませんでした。接続情報を確認してください。", innerException: ex);
        }

        return connection;
    }

    private static async Task EnsureSchemaAsync(MySqlConnection connection, CancellationToken cancellationToken)
    {
        if (_schemaInitialized)
        {
            return;
        }

        await SchemaLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_schemaInitialized)
            {
                return;
            }

            const string groupsSql = @"CREATE TABLE IF NOT EXISTS term_groups (
                id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                description VARCHAR(500) NULL,
                parent_group_id INT NULL,
                created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                CONSTRAINT fk_parent_group FOREIGN KEY (parent_group_id) REFERENCES term_groups(id) ON DELETE SET NULL
            ) ENGINE=InnoDB;";

            await using (var createGroups = new MySqlCommand(groupsSql, connection))
            {
                await createGroups.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            const string termsSql = @"CREATE TABLE IF NOT EXISTS terms (
                id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(200) NOT NULL,
                definition TEXT NULL,
                group_id INT NULL,
                created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                CONSTRAINT fk_terms_group FOREIGN KEY (group_id) REFERENCES term_groups(id) ON DELETE SET NULL
            ) ENGINE=InnoDB;";

            await using (var createTerms = new MySqlCommand(termsSql, connection))
            {
                await createTerms.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            _schemaInitialized = true;
        }
        finally
        {
            SchemaLock.Release();
        }
    }

    public async Task<IReadOnlyList<GroupSummary>> GetGroupSummariesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

            const string sql = @"SELECT g.id,
                       g.name,
                       g.description,
                       g.parent_group_id,
                       p.name AS parent_name,
                       COUNT(DISTINCT sg.id) AS sub_group_count,
                       COUNT(DISTINCT t.id) AS term_count,
                       g.created_at
                  FROM term_groups AS g
             LEFT JOIN term_groups AS sg ON sg.parent_group_id = g.id
             LEFT JOIN term_groups AS p ON g.parent_group_id = p.id
             LEFT JOIN terms AS t ON t.group_id = g.id
              GROUP BY g.id, g.name, g.description, g.parent_group_id, parent_name, g.created_at
              ORDER BY g.name ASC;";

            await using var command = new MySqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            var groups = new List<GroupSummary>();
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                groups.Add(new GroupSummary
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                    ParentGroupId = reader.IsDBNull("parent_group_id") ? null : reader.GetInt32("parent_group_id"),
                    ParentGroupName = reader.IsDBNull("parent_name") ? null : reader.GetString("parent_name"),
                    SubGroupCount = reader.GetInt32("sub_group_count"),
                    TermCount = reader.GetInt32("term_count"),
                    CreatedAt = reader.GetDateTime("created_at")
                });
            }

            return groups;
        }
        catch (DataAccessException)
        {
            throw;
        }
        catch (MySqlException ex)
        {
            throw new DataAccessException("グループ情報の取得中にエラーが発生しました。", innerException: ex);
        }
    }

    public async Task<IReadOnlyList<GroupOption>> GetParentGroupOptionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

            const string sql = "SELECT id, name FROM term_groups ORDER BY name ASC;";
            await using var command = new MySqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            var groups = new List<GroupOption>();
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                groups.Add(new GroupOption(reader.GetInt32("id"), reader.GetString("name")));
            }

            return groups;
        }
        catch (DataAccessException)
        {
            throw;
        }
        catch (MySqlException ex)
        {
            throw new DataAccessException("親グループ候補の取得中にエラーが発生しました。", innerException: ex);
        }
    }

    public async Task CreateGroupAsync(GroupInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        try
        {
            await using var connection = await CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

            var normalizedName = (input.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new DataAccessException("グループ名を入力してください。");
            }

            var normalizedDescription = string.IsNullOrWhiteSpace(input.Description)
                ? null
                : input.Description.Trim();

            const string duplicateCheckSql = @"SELECT COUNT(*)
                  FROM term_groups
                 WHERE name = @name
                   AND (parent_group_id <=> @parentId);";

            await using (var duplicateCommand = new MySqlCommand(duplicateCheckSql, connection))
            {
                duplicateCommand.Parameters.AddWithValue("@name", normalizedName);
                duplicateCommand.Parameters.AddWithValue("@parentId", (object?)input.ParentGroupId ?? DBNull.Value);

                var existingCount = Convert.ToInt32(await duplicateCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
                if (existingCount > 0)
                {
                    throw new DataAccessException("同じ名前のグループが既に存在します。別の名前を入力してください。");
                }
            }

            const string insertSql = @"INSERT INTO term_groups (name, description, parent_group_id)
                VALUES (@name, @description, @parentId);";

            await using var insertCommand = new MySqlCommand(insertSql, connection);
            insertCommand.Parameters.AddWithValue("@name", normalizedName);
            insertCommand.Parameters.AddWithValue("@description", (object?)normalizedDescription ?? DBNull.Value);
            insertCommand.Parameters.AddWithValue("@parentId", (object?)input.ParentGroupId ?? DBNull.Value);

            await insertCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DataAccessException)
        {
            throw;
        }
        catch (MySqlException ex)
        {
            throw new DataAccessException("グループの作成中にエラーが発生しました。", innerException: ex);
        }
    }

    public async Task<TermSummary> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

            const string sql = @"SELECT COUNT(*) AS total_terms,
                       SUM(CASE WHEN TRIM(COALESCE(definition, '')) = '' THEN 1 ELSE 0 END) AS outstanding_terms,
                       SUM(CASE WHEN group_id IS NULL THEN 1 ELSE 0 END) AS terms_without_group,
                       MAX(updated_at) AS last_updated
                  FROM terms;";

            await using var command = new MySqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var total = reader.IsDBNull("total_terms") ? 0 : Convert.ToInt32(reader["total_terms"]);
                var outstanding = reader.IsDBNull("outstanding_terms") ? 0 : Convert.ToInt32(reader["outstanding_terms"]);
                var withoutGroup = reader.IsDBNull("terms_without_group") ? 0 : Convert.ToInt32(reader["terms_without_group"]);
                DateTime? lastUpdated = reader.IsDBNull("last_updated") ? null : reader.GetDateTime("last_updated");

                return new TermSummary(total, outstanding, withoutGroup, lastUpdated);
            }

            return TermSummary.Empty;
        }
        catch (DataAccessException)
        {
            throw;
        }
        catch (MySqlException ex)
        {
            throw new DataAccessException("用語情報の取得中にエラーが発生しました。", innerException: ex);
        }
    }

    public DataTable GetSummaries()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new DataAccessException(
                "データベース接続文字列が設定されていません。appsettings.json で ConnectionStrings:TermAppDb を設定してください。",
                isConfigurationError: true);
        }

        var table = new DataTable();

        using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        const string sql = "SELECT news, created_at FROM day_news ORDER BY created_at DESC LIMIT 10;";

        using var cmd = new MySqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        table.Load(reader);

        return table;
    }

    public class DataAccessException : Exception
    {
        public bool IsConfigurationError { get; }

        public DataAccessException(string message, Exception? innerException = null, bool isConfigurationError = false)
            : base(message, innerException)
        {
            IsConfigurationError = isConfigurationError;
        }
    }

    public class GroupInput
    {
        [Required(ErrorMessage = "グループ名を入力してください。")]
        [StringLength(100, ErrorMessage = "グループ名は100文字以内で入力してください。")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "説明は500文字以内で入力してください。")]
        public string? Description { get; set; }

        public int? ParentGroupId { get; set; }
    }

    public record GroupOption(int Id, string Name);

    public record GroupSummary
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int? ParentGroupId { get; init; }
        public string? ParentGroupName { get; init; }
        public int SubGroupCount { get; init; }
        public int TermCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record TermSummary(int TotalTerms, int OutstandingTerms, int TermsWithoutGroup, DateTime? LastUpdated)
    {
        public static TermSummary Empty { get; } = new(0, 0, 0, null);
    }
}
