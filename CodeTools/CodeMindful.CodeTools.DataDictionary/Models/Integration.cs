using System.Collections.Generic;

namespace CodeMindful.CodeTools.DataDictionary.Models;

/// <summary>Integration is something like an SSIS/DTSX package, C#, PowerShell related to a Database</summary>
public class Integration : IDescription
{
    public int IntegrationId { get; set; }

    /// <summary>Name of Integration (SSIS Package Name, or File Name?)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>What kind of Integration Package (SSIS, C#, Etc)</summary>
    public string IntegrationType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public virtual HashSet<IntegrationObject> IntegrationObjects { get; set; } = [];
}

/// <summary>Connect an Integration to a SQL Object</summary>
public class IntegrationObject : ISqlObjectId, IDescription
{
    /// <summary>FKey to Integration</summary>
    public int IntegrationId { get; set; }

    /// <summary>Integration Referenced by Id</summary>
    public virtual Integration? Integration { get; set; }

    public int SqlObjectId { get; set; }

    public virtual SqlObject? SqlObject { get; set; }

    /// <summary>CRUD Operation</summary>
    public string? Operation { get; set; }

    public string Description { get; set; } = string.Empty;
}
