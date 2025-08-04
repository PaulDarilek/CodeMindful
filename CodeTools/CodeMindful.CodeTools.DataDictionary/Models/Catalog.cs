using Microsoft.EntityFrameworkCore;

namespace CodeMindful.CodeTools.DataDictionary.Models;

public interface ICatalogName
{
    public string CatalogName { get; }
}


/// <summary>A Catalog is a Database</summary>
[PrimaryKey(nameof(CatalogName))]
public class Catalog : ICatalogName, IDescription
{
    /// <summary>Name of the Database</summary>
    public string CatalogName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

}
