using Azure.Core.Pipeline;
using Microsoft.EntityFrameworkCore;

namespace CodeMindful.CodeTools.DataDictionary.Models;


public interface IServer
{
    /// <summary>Name of the Database</summary>
    public string ServerName { get; set; }
}

[PrimaryKey(nameof(ServerName))]
public class Server : IServer, IDescription
{
    /// <summary>Name of the Database</summary>
    public string ServerName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Dev, Test, Staging/Integration, Production</summary>
    public string Environment { get; set; } = string.Empty;

}
