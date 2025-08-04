using CodeMindful.CodeTools.DataDictionary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeMindful.CodeTools.DataDictionary;

/// <summary>Constructor</summary>
/// <param name="dataDictionary"></param>
public class ReportToMarkDown(DataContext dataDictionary, DirectoryInfo reportDirectory)
{
    private DataContext DataDictionary { get; } = dataDictionary;
    private DirectoryInfo ReportDirectory { get; } = reportDirectory;

    private const StringComparison IgnoreCase = System.StringComparison.OrdinalIgnoreCase;

    public void GenerateMarkdown(string? catalogName = null)        
    {
        var catalogs =
            string.IsNullOrEmpty(catalogName) ?
            [.. DataDictionary.Catalogs] :
            DataDictionary.Catalogs.Where(c => c.CatalogName == catalogName).ToList();

        foreach (var catlog in catalogs)
        {
            GenerateMarkDown(catlog);
        }
    }

    public void GenerateMarkDown(Catalog catalog)
    {
        ReportDirectory.Create();

        var text = new StringBuilder()
            .AppendLine("[Home](.)")
            .AppendLine("[[_TOC_]]")
            .AppendLine();

        AppendCatalog(catalog, text);
        AppendTables(catalog, text, isView: false);
        AppendTables(catalog, text, isView: true);
        AppendRoutines(catalog, text);

        var report = new FileInfo(Path.Combine(ReportDirectory.FullName, catalog.CatalogName + ".Database.md"));
        report.Delete();
        File.WriteAllText(report.FullName, text.ToString());
        Console.WriteLine($"Report: {report.FullName}");
    }

    public void GenerateDependencyGraph(SqlObject sqlObject, StringBuilder text)
    {
        var thisSchema = $"{sqlObject.SchemaName}[{sqlObject.ObjectName}]";

        // Generate Links
        var referencedBy = GetParentReferences(sqlObject);
        var references = GetChildReferences(sqlObject);

        if (referencedBy.Count == 0 && references.Count == 0)
        {
            return;
        }

        if (referencedBy.Count > 0)
        {
            var links = referencedBy.Select(x => MakeLink(x.SqlObject!));
                
            text.Append(" - referenced by: ").AppendLine(string.Join(", ", links));
        }
        if (references.Count > 0)
        {
            var links =
                references
                .Select(item => MakeLink(item.ReferencedObject!));

            text.Append(" - references: ").AppendLine(string.Join(", ", links));
        }

        // Generate Dependency Diagram
        text
            .AppendLine("::: mermaid")
            .AppendLine("graph LR;");

        foreach (var item in referencedBy.Union(references))
        {
            string source = item.SqlObject!.TwoPartName();
            string target = item.ReferencedObject!.TwoPartName();
            text.AppendLine($"{source} --> {target} ");
        }

        text.AppendLine(":::");
        text.AppendLine();

        static string MakeLink(SqlObject sqlObject)
        {
            var name = sqlObject.TwoPartName();
            return $"[{name}](#{name.Replace(' ', '-')})";
        }
    }

    /// <summary>Set of parent sql objects (views/procs) that references a specific object</summary>
    private HashSet<SqlDependency> GetParentReferences(SqlObject sqlObject)
    {
        var result = DataDictionary.SqlDependencies
            .Include(x => x.ReferencedObject)
            .Include(x => x.SqlObject)
            .Where(row => row.ReferencedObjectId == sqlObject.SqlObjectId)
            .OrderBy(row => row.SqlObject!.SchemaName)
            .ThenBy(row => row.SqlObject!.ObjectName)
            .ToHashSet();
        return result;
    }

    /// <summary>Set of child sql objects (tables/procs) that an object references</summary>
    private HashSet<SqlDependency> GetChildReferences(SqlObject sqlObject)
    {
        var result = DataDictionary.SqlDependencies
            .Include(x => x.ReferencedObject)
            .Include(x => x.SqlObject)
            .Where(row => row.SqlObjectId == sqlObject.SqlObjectId)
            .OrderBy(row => row.ReferencedObject!.SchemaName)
            .ThenBy(row => row.ReferencedObject!.ObjectName)
            .ToHashSet();

        return result;
    }

    private void AppendCatalog(Catalog catalog, StringBuilder text)
    {
        var servers = DataDictionary.ServerCatalogs
            .Where(row => row.CatalogName == catalog.CatalogName)
            .Select(row => row.ServerName)
            .Distinct()
            .ToList();
        text.Append("# ").AppendLine(catalog.CatalogName);

        text.AppendLine($"- Description: {catalog.Description}|");

        text.AppendLine($"- Servers: {string.Join(", ", servers)}");

        text.AppendLine();
    }

    private void AppendTables(Catalog catalog, StringBuilder text, bool isView)
    {
        var sqlType = isView ? SqlType.View : SqlType.Table;

        var tables = DataDictionary.SqlObjects
            .Where(x => x.CatalogName == catalog.CatalogName && x.Type == sqlType)
            .OrderBy(x => x.SchemaName)
            .ThenBy(x => x.ObjectName)
            .ToList();

        text.AppendLine(isView ? "# Views" : "# Tables");

        foreach (var table in tables)
        {
            text
                .AppendLine($"## {table.TwoPartName()}")
                .AppendLine($" - {table.Type} (Id={table.SqlObjectId})")
                ;

            GenerateDependencyGraph(table, text);
            AppendColumns(table, text);
        }

        text.AppendLine();
    }
    
    private void AppendColumns(SqlObject table, StringBuilder text)
    {
        var cols = 
            DataDictionary.Columns
            .Where(x => x.SqlObjectId == table.SqlObjectId)
            .OrderBy(c => c.Ordinal)
            .ToList();

        text
            .AppendLine()
            .AppendLine("|#|Name|DataType|IsNullable|Default|")
            .AppendLine("|--|--|--|--|--|");

        foreach (var column in cols) 
        {
            text.AppendLine($"|{column.Ordinal}|{column.ColumnName}|{column.DataType}|{column.IsNullable}|{column.Default}|");
        }
        text.AppendLine();

    }

    private void AppendRoutines(Catalog catalog, StringBuilder text)
    {
        var routines = DataDictionary.SqlObjects
            .Where(x => x.CatalogName == catalog.CatalogName && (x.Type == SqlType.Procedure || x.Type == SqlType.Function) )
            .OrderBy(x => x.SchemaName)
            .ThenBy(x => x.ObjectName)
            .ToList();

        text
            .AppendLine("# Routines");

        foreach (var routine in routines)
        {
            var key = $"{routine.SchemaName}.{routine.ObjectName}";

            text
                .AppendLine($"## {key}")
                .AppendLine($" - {routine.Type} (Id={routine.SqlObjectId} {routine.Operation} {routine.Data_Type})");

            GenerateDependencyGraph(routine, text);
        }
        text.AppendLine();
    }

}
