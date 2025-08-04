using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMindful.CodeTools.DataDictionary.Models;


public interface ISqlObjectId
{
    public int SqlObjectId { get; set; }
}
public interface IObjectName
{
    public string ObjectName { get; set; }
}

[PrimaryKey(nameof(SqlObjectId))]
public class SqlObject : ISqlObjectId, ICatalogSchema, IObjectName
{
    public int SqlObjectId { get; set; }

    public string CatalogName { get; set; } = string.Empty;

    public string SchemaName { get; set; } = string.Empty;

    public string ObjectName { get; set; } = string.Empty;

    public SqlType Type { get; set; }

    /// <summary>Return Type of Function</summary>
    public string? Data_Type { get; set; }

    /// <summary>Does Proc do CRUD?</summary>
    public string? Operation { get; set; }

    /// <summary>Text Description of purpose or context (manually input)</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Script for Stored Procedures or Functions</summary>
    public string? Definition { get; set; }

    public string TwoPartName() => string.IsNullOrEmpty(SchemaName) ? ObjectName : $"{SchemaName}.{ObjectName}";
    public string ThreePartName() => $"{CatalogName}.{SchemaName}.{ObjectName}";

    //public virtual DbSet<SqlDependency>? ObjectsReferenced { get; set; }
    //public virtual DbSet<SqlDependency>? ObjectsReferencing { get; set; }
    //public virtual DbSet<IntegrationObject>? IntegrationObjects { get; set; }

}
