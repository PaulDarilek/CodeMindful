using CodeMindful.CodeTools.DataDictionary.Models;
using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Column = CodeMindful.CodeTools.DataDictionary.Models.Column;
using System.Diagnostics;

namespace CodeMindful.CodeTools.DataDictionary;

public class SqlServerScanner : IDisposable
{
    private DataContext DataDictionary { get; }
    internal SqlConnection? Connection { get; set; }

    /// <summary>Constructor</summary>
    /// <param name="connectionString"></param>
    public SqlServerScanner(DataContext dataDict)
    {
        ArgumentNullException.ThrowIfNull(dataDict);
        DataDictionary = dataDict;
    }

    public async Task ReadInformationSchema(SqlConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        Connection = connection;
        await ReadCatalogName();
        await ReadSchemas();
        await ReadTables();
        await ReadColumns();
        await ReadRoutines();
        await ReadDependencies();
        await RemoveSystemObjects();
    }

    public async Task RemoveSystemObjects()
    {
        await RunSqliteCmd("Delete from SqlDependencies where ReferencedSchema in ('dbo','') and ReferencedObjectName in ('dtproperties','sysdiagrams', 'sysproperties');");
        await RunSqliteCmd("Delete from Tables where Schema = 'dbo' and TableName in ('dtproperties','sysdiagrams', 'sysproperties');");
        await RunSqliteCmd("Delete from Routines where RoutineName like 'dt_%';");
        await RunSqliteCmd("Delete from SqlDependencies where ObjectName like 'sp_%diagram%';");
        await RunSqliteCmd("Delete from Routines where RoutineName like 'sp_%diagram%';");

        async Task<int> RunSqliteCmd(string sql)
        {
            Debug.Assert(!string.IsNullOrEmpty(sql));
            int count = 0;
            //count += await DataDictionary.Database.ExecuteSqlRawAsync(sql);
            count += await DataDictionary.SaveChangesAsync();
            return count;
        }
    }

    internal async Task ReadCatalogName()
    {
        using var reader = await RunQueryAsync("Select DB_NAME() as Catalog, @@ServerName as ServerName");
        if (reader == null) return;

        while (await reader.ReadAsync())
        {
            var catalog = new Catalog
            {
                CatalogName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
            };

            var server = new Server
            {
                ServerName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
            };

            if (!DataDictionary.Servers.Any(row => row.ServerName == server.ServerName))
            {
                DataDictionary.Servers.Add(server);
            }

            if (!DataDictionary.Catalogs.Any(row => row.CatalogName == catalog.CatalogName))
            {
                DataDictionary.Catalogs.Add(catalog);
            }

            if (!DataDictionary.ServerCatalogs.Any(s => s.ServerName == server.ServerName && s.CatalogName == catalog.CatalogName ))
            {
                DataDictionary.ServerCatalogs.Add(new ServerCatalog { ServerName = server.ServerName, CatalogName = catalog.CatalogName});
            }

        }
        await DataDictionary.SaveChangesAsync();
    }

    internal async Task ReadSchemas()
    {
        using var reader = await RunQueryAsync("Select SCHEMA_NAME, CATALOG_NAME, SCHEMA_OWNER from INFORMATION_SCHEMA.SCHEMATA");
        if (reader == null) return;

        while (await reader.ReadAsync())
        {
            var row = new Schema { 
                SchemaName = reader.GetString(0).ToLower(), 
                CatalogName = reader.GetString(1),
                Owner = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            };

            var match = DataDictionary.Schemas.Where(c => c.SchemaName == row.SchemaName && c.CatalogName == row.CatalogName ).FirstOrDefault();
            if (match == null)
            {
                DataDictionary.Schemas.Add(row);
            }
        }
        await DataDictionary.SaveChangesAsync();
    }

    internal async Task ReadTables()
    {
        using var reader = await RunQueryAsync("Select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES");
        if (reader == null) return;

        while (await reader.ReadAsync())
        {
            var row = new SqlObject
            {
                CatalogName = reader.GetString(0),
                SchemaName = reader.GetString(1).ToLower(),
                ObjectName = reader.GetString(2),
                Type = (reader.GetString(3) == "VIEW") ? SqlType.View : SqlType.Table, 
            };

            if (IsSystemObject(row.SchemaName, row.ObjectName)) continue;

            var match = DataDictionary.SqlObjects.Where(c => c.CatalogName == row.CatalogName && c.SchemaName == row.SchemaName && c.ObjectName == row.ObjectName ).FirstOrDefault();
            if (match == null)
            {
                DataDictionary.SqlObjects.Add(row);
            }
        }
        await DataDictionary.SaveChangesAsync();
    }

    internal async Task ReadColumns()
    {
        const string sql =
            "SELECT TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,ORDINAL_POSITION" +
            ",COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH" +
            " FROM INFORMATION_SCHEMA.COLUMNS";
        using var reader = await RunQueryAsync(sql);
        if (reader == null) return;

        while (await reader.ReadAsync())
        {
            string catalog = reader.GetString(0);
            string schema = reader.GetString(1).ToLower();
            string tableName = reader.GetString(2);

            if (IsSystemObject(schema, tableName)) continue;

            var table =
                DataDictionary.SqlObjects.Where(x => x.CatalogName == catalog && x.SchemaName == schema && x.ObjectName == tableName).FirstOrDefault();

            if (table == null)
            {
                Console.WriteLine($"Missing Table: {catalog}.{schema}.{tableName}");
                continue;
            }

            var row = new Column
            {
                SqlObjectId = table.SqlObjectId,
                Table = table,
                ColumnName = reader.GetString(3),
                Ordinal = reader.GetInt32(4),
                Default = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                IsNullable = (reader.GetString(6) != "NO"),
                DataType =
                    reader.GetString(7) +
                    (reader.IsDBNull(8) ? string.Empty : $"({reader.GetInt32(8)})"),
            };
                        
            var match = DataDictionary.Columns
                .Where(c => c.SqlObjectId == row.SqlObjectId && c.Ordinal == row.Ordinal)
                .FirstOrDefault();

            if (match == null)
            {
                DataDictionary.Columns.Add(row);
            }
            else
            {
                if (match.ColumnName != row.ColumnName) match.ColumnName = row.ColumnName;
                if (match.Default != row.Default) match.Default = row.Default;
                if (match.IsNullable != row.IsNullable) match.IsNullable = row.IsNullable;
                if (match.DataType != row.DataType) match.DataType = row.DataType;
            }
        }
        await DataDictionary.SaveChangesAsync();
    }

    internal async Task ReadRoutines()
    {
        const string sql = "SELECT ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, ROUTINE_DEFINITION FROM INFORMATION_SCHEMA.ROUTINES";
        using var reader = await RunQueryAsync(sql);
        if (reader == null) return;

        while (await reader.ReadAsync())
        {
            var row = new SqlObject
            {
                SqlObjectId = default,
                CatalogName = reader.GetString(0),
                SchemaName = reader.GetString(1).ToLower(),
                ObjectName = reader.GetString(2),
                Type = (reader.GetString(3) == "FUNCTION") ? SqlType.Function : SqlType.Procedure,
                Data_Type =
                 (reader.IsDBNull(4) ? string.Empty : reader.GetString(4)) +
                 (reader.IsDBNull(5) ? string.Empty : $"({reader.GetInt32(5)})"),
                Definition = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
            };

            if (IsSystemObject(row.SchemaName, row.ObjectName)) continue;

            var match = DataDictionary.SqlObjects.Where(c => c.ObjectName == row.ObjectName && c.CatalogName == row.CatalogName && c.SchemaName == row.SchemaName).FirstOrDefault();
            if (match == null)
            {
                DataDictionary.SqlObjects.Add(row);
            }
        }
        await DataDictionary.SaveChangesAsync();
    }

    internal async Task ReadDependencies()
    {
        string sql = "SELECT " +
            "DB_Name() as CatalogName" +
            ", OBJECT_SCHEMA_NAME(referencing_id) as [Schema]" +
            ", OBJECT_NAME(referencing_id) AS ObjectName" +
            ", referenced_schema_name" +
            ", referenced_entity_name" +
            " FROM sys.sql_expression_dependencies" +
            " WHERE (OBJECTPROPERTY(referencing_id, 'IsProcedure') = 1 OR OBJECTPROPERTY(referencing_id, 'IsView') = 1)" +
            " ORDER BY OBJECT_SCHEMA_NAME(referencing_id), OBJECT_NAME(referencing_id), referenced_schema_name, referenced_entity_name";

        using var reader = await RunQueryAsync(sql);
        if (reader == null) return;

        while (await reader.ReadAsync())
        {
            string catalogName = reader.GetString(0);
            string schemaName = reader.GetString(1);
            string objectName = reader.GetString(2);
            string referencedSchemaName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            string referencedObjectName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

            if (IsSystemObject(schemaName, objectName)) continue;
            if (IsSystemObject(referencedSchemaName, referencedObjectName)) continue;

            var sqlObject =
                DataDictionary.SqlObjects
                .Where(x => x.CatalogName == catalogName && x.SchemaName == schemaName && x.ObjectName == objectName)
                .FirstOrDefault();

            if (sqlObject == null)
            {
                Console.WriteLine($"Can't Find {nameof(SqlObject)}: {catalogName}.{schemaName}.{objectName}");
                continue;
            }

            referencedSchemaName =
                DataDictionary.Schemas
                .Where(x => x.CatalogName == catalogName && x.SchemaName == referencedSchemaName).Select(x => x.SchemaName)
                .FirstOrDefault() ?? referencedSchemaName;

            var referencedObject =
                DataDictionary.SqlObjects
                .Where(x => x.CatalogName == catalogName && x.SchemaName == referencedSchemaName && x.ObjectName == referencedObjectName)
                .FirstOrDefault();

            if (referencedObject == null)
            {
                var error =
                    DataDictionary.SqlDependencyErrors
                    .Where(x => x.SqlObjectId == sqlObject.SqlObjectId
                    && x.ReferencedSchemaName == referencedSchemaName
                    && x.ReferencedObjectName == referencedObjectName)
                    .FirstOrDefault();

                if (error == null)
                {
                    error = new SqlDependencyError
                    {
                        Id = default,
                        SqlObjectId = sqlObject.SqlObjectId,
                        SqlObject = sqlObject,
                        CatalogName = catalogName,
                        SchemaName = schemaName,
                        ObjectName = objectName,
                        ReferencedSchemaName = referencedSchemaName,
                        ReferencedObjectName = referencedObjectName,
                        ReferencedObjectId = default,
                        ReferencedObject = default,
                    };

                    DataDictionary.SqlDependencyErrors.Add(error);
                    await DataDictionary.SaveChangesAsync();
                    Console.WriteLine($"{sqlObject.CatalogName}.{sqlObject.SchemaName}.{sqlObject.ObjectName} --unknown reference--> {error.ReferencedSchemaName}.{error.ReferencedObjectName}");
                }
                continue;
            }

            var match =
                DataDictionary.SqlDependencies
                .Where(c => c.SqlObjectId == sqlObject.SqlObjectId && c.ReferencedObjectId == referencedObject.SqlObjectId)
                .FirstOrDefault();
            
            if (match == null)
            {
                var row = new Models.SqlDependency
                {
                    SqlDependencyId = default,
                    SqlObjectId = sqlObject.SqlObjectId,
                    SqlObject = sqlObject,
                    ReferencedObject = referencedObject,
                    ReferencedObjectId = referencedObject.SqlObjectId,
                };
                DataDictionary.SqlDependencies.Add(row);
            }
        }
        await DataDictionary.SaveChangesAsync();

    }

    private async Task<DbDataReader> RunQueryAsync(string sql)
    {
        using var cmd = new SqlCommand(sql, Connection)
        {
            CommandText = sql,
            CommandType = System.Data.CommandType.Text,
            CommandTimeout = 90,
        };
        try
        {
            var reader = await cmd.ExecuteReaderAsync();
            return reader;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"sql\n{sql}\n{ex.Message}");
            throw;
        }
    }


    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Dispose();
            Connection = null;
        }
        GC.SuppressFinalize(this);
    }

    private static bool IsSystemObject(string schema, string objectName)
    {
        schema = schema?.ToLower() ?? string.Empty;
        objectName = objectName?.ToLower() ?? string.Empty;

        if (schema == "dbo" || schema == string.Empty)
        {
            if (objectName == "dtproperties" || 
                objectName == "sysdiagrams" || 
                objectName == "sysproperties") return true;
            if (objectName.StartsWith("dt_")) return true;
            if (objectName.StartsWith("sp_") && objectName.Contains("diagram")) return true;
        }
        return false;
    }
}
