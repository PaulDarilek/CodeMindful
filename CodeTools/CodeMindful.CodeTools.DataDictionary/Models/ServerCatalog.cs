using System.Diagnostics;

namespace CodeMindful.CodeTools.DataDictionary.Models;

[DebuggerStepThrough]
public class ServerCatalog : IServer, ICatalogName
{
    public string ServerName { get; set; } = string.Empty;

    public string CatalogName { get; set; } = string.Empty;

    /// <summary>Server</summary>
    public virtual Server? Server { get; set; }

    /// <summary>Catalog</summary>
    public virtual Catalog? Catalog { get; set; }

}
