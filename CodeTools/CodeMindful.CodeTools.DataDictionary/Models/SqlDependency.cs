namespace CodeMindful.CodeTools.DataDictionary.Models;

/// <summary>Tables depended on by a Routine</summary>
public class SqlDependency 
{
    public int SqlDependencyId { get; set; }

    public int SqlObjectId { get; set; }

    public virtual SqlObject? SqlObject { get; set; }

    public int ReferencedObjectId { get; set; }

    public virtual SqlObject? ReferencedObject {get; set;}

    /// <summary>Type of Operation (Exec, Read, Insert, Update, Delete)</summary>
    public string? Operation { get; set; }

    /// <summary>Description of the Reference</summary>
    public string Description { get; set; } = string.Empty;

}

