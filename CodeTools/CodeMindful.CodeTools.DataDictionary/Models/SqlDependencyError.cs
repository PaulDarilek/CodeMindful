using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CodeMindful.CodeTools.DataDictionary.Models;

/// <summary>Troubles Importing Sql Dependencies</summary>
public class SqlDependencyError
{
    public int Id { get; set; }
    public int SqlObjectId { get; set; }

    public string CatalogName { get; set; } = string.Empty;

    public string SchemaName { get; set; } = string.Empty;

    public string ObjectName { get; set; } = string.Empty;

    public virtual SqlObject? SqlObject { get; set; }

    public string? ReferencedCatalogName { get; set; }

    public string? ReferencedSchemaName { get; set; }

    public string? ReferencedObjectName { get; set; }

    public int? ReferencedObjectId { get; set; }

    public virtual SqlObject? ReferencedObject { get; set; }

}

