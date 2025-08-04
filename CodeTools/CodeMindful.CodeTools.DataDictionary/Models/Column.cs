using Microsoft.EntityFrameworkCore;

namespace CodeMindful.CodeTools.DataDictionary.Models;

[PrimaryKey(nameof(SqlObjectId), nameof(Ordinal))]
public class Column : ISqlObjectId, IDescription
{
    /// <summary>Id of Table</summary>
    public int SqlObjectId { get; set; }

    /// <summary>Table or View</summary>
    public  virtual SqlObject? Table { get; set; }

    /// <summary>Name of the Column</summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>Column Number</summary>
    public int? Ordinal { get; set; }

    public bool? IsNullable { get; set; }

    public string DataType { get; set; } = string.Empty;

    public string Default { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
