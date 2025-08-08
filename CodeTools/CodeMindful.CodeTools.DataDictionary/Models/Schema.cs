namespace CodeMindful.CodeTools.DataDictionary.Models;

public interface ISchema
{
    public string SchemaName { get; }
}

public interface ICatalogSchema : ICatalogName, ISchema
{
}


public class Schema : ICatalogSchema, IDescription
{
    public string CatalogName { get; set; } = string.Empty;

    /// <summary>Name of the Schema</summary>
    public string SchemaName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public string? Owner { get; set; }

}
