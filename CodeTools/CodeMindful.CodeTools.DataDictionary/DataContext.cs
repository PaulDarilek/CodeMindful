using CodeMindful.CodeTools.DataDictionary.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CodeMindful.CodeTools.DataDictionary;

public class DataContext : DbContext
{
    private static bool _migrated;

    public string ConnectionString { get; }

    /// <summary>Sql Servers</summary>
    public virtual DbSet<Server> Servers { get; set; }

    /// <summary>Databases</summary>
    public virtual DbSet<Catalog> Catalogs { get; set; }

    /// <summary>Catalogs on a Sql Server</summary>
    public virtual DbSet<ServerCatalog> ServerCatalogs { get; set; }

    /// <summary>Schemas in Database</summary>
    public virtual DbSet<Schema> Schemas { get; set; }

    /// <summary>Tables/View/Procedures/Functions in Database</summary>
    public virtual DbSet<SqlObject> SqlObjects { get; set; }

    /// <summary>Track Routine's Dependency on another Object</summary>
    public virtual DbSet<SqlDependency> SqlDependencies { get; set; }


    /// <summary>Track Routine's Dependency on another Object</summary>
    public virtual DbSet<SqlDependencyError> SqlDependencyErrors { get; set; }


    /// <summary>Columns in Table or View</summary>
    public virtual DbSet<Column> Columns { get; set; }

    /// <summary>Integrations (External?) such as SSIS, PowerShell, C#</summary>
    public virtual DbSet<Integration> Integrations { get; set; }

    /// <summary>Track Integration's Dependency on a Table</summary>
    public virtual DbSet<IntegrationObject> IntegrationObjects { get; set; }

    [Obsolete("Used for EF Migrations Only", error: true)]
    public DataContext() : base()
    {
        ConnectionString = "Data Source=%USERPROFILE%\\Sqlite\\DataDictionary.db";
    }

    public DataContext(string connectionString) : base()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ConnectionString = connectionString;
        MigrateSchemaChanges();
    }

    public void MigrateSchemaChanges()
    {
        if (!_migrated)
        {
            base.Database.Migrate();
            base.SaveChanges(true);
            _migrated = true;
        }
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        ModelHelper.OnConfiguring(options, ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new ModelHelper().ConfigureModels(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }




}
