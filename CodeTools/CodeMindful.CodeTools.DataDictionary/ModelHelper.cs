using CodeMindful.CodeTools.DataDictionary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace CodeMindful.CodeTools.DataDictionary;

internal class ModelHelper :
    IEntityTypeConfiguration<Server>, 
    IEntityTypeConfiguration<Catalog>,
    IEntityTypeConfiguration<ServerCatalog>, 
    IEntityTypeConfiguration<Schema>,
    IEntityTypeConfiguration<Column>,
    IEntityTypeConfiguration<SqlObject>,
    IEntityTypeConfiguration<SqlDependency>,
    IEntityTypeConfiguration<SqlDependencyError>,
    IEntityTypeConfiguration<Integration>, 
    IEntityTypeConfiguration<IntegrationObject>
{
    public static string? Schema { get; set; }

    public static void OnConfiguring(DbContextOptionsBuilder options, string connectionString)
    {
#if Sqlite
        options.UseSqlite(ConnectionString);
        Schema = null;
#else
        options.UseSqlServer(connectionString);
        Schema = "dbo";
#endif
    }

    public void ConfigureModels(ModelBuilder modelBuilder)
    {
#if Sqlite
        modelBuilder.UseCollation("NOCASE");
#endif

        var props =
            modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string));

#if Sqlite
        foreach (var property in props)
        {
            property.SetCollation("NOCASE");
        }
#endif

        Configure(modelBuilder.Entity<Server>());
        Configure(modelBuilder.Entity<Catalog>());
        Configure(modelBuilder.Entity<ServerCatalog>());
        Configure(modelBuilder.Entity<Schema>());
        Configure(modelBuilder.Entity<Column>());

        Configure(modelBuilder.Entity<SqlObject>());
        Configure(modelBuilder.Entity<SqlDependency>());
        
        Configure(modelBuilder.Entity<Integration>());
        Configure(modelBuilder.Entity<IntegrationObject>());
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        string tableName = nameof(DataContext.Servers);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => x.ServerName)
            .HasName("PK_" + tableName);
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        string tableName = nameof(DataContext.Catalogs);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => x.CatalogName)
            .HasName("PK_" + tableName);

    }

    public void Configure(EntityTypeBuilder<ServerCatalog> builder)
    {
        string tableName = nameof(DataContext.ServerCatalogs);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => new { x.ServerName, x.CatalogName})
            .HasName("PK_" + tableName);

        builder.HasOne(x => x.Server)
            .WithMany()
            .HasForeignKey(x => x.ServerName)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasOne(x => x.Catalog)
            .WithMany()
            .HasForeignKey(x => x.CatalogName)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<Schema> builder)
    {
        string tableName = nameof(DataContext.Schemas);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => new { x.CatalogName, x.SchemaName})
            .HasName("PK_" + tableName);
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<SqlObject> builder)
    {
        string tableName = nameof(DataContext.SqlObjects);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => x.SqlObjectId)
            .HasName("PK_" + tableName);

        builder.HasIndex([
            //nameof(SqlObject.ServerName),
            nameof(SqlObject.CatalogName),
            nameof(SqlObject.SchemaName),
            nameof(SqlObject.ObjectName),
        ], "QualifiedName").IsUnique(true);

        builder.Property(x => x.Type)
            .HasConversion
            (
                value => value.ToString(),
                value => Enum.Parse<SqlType>(value)
            );

        builder.HasIndex([nameof(SqlObject.ObjectName)], nameof(SqlObject.ObjectName)).IsUnique(false);
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<SqlDependency> builder)
    {
        string tableName = nameof(DataContext.SqlDependencies);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => x.SqlDependencyId)
            .HasName("PK_" + tableName);

        builder.HasOne(x => x.SqlObject)
            .WithMany()
            .HasForeignKey(x => x.SqlObjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(true);

        builder.HasOne(x => x.ReferencedObject)
            .WithMany()
            .HasForeignKey(x => x.ReferencedObjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<SqlDependencyError> builder)
    {
        string tableName = nameof(DataContext.SqlDependencies);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => x.Id)
            .HasName("PK_" + tableName);

        builder.HasIndex([nameof(SqlDependencyError.SqlObjectId)], nameof(SqlDependencyError.SqlObjectId));
        builder.HasIndex([nameof(SqlDependencyError.ReferencedObjectId)], nameof(SqlDependencyError.ReferencedObjectId));
        builder.HasIndex([nameof(SqlDependencyError.ReferencedSchemaName), nameof(SqlDependencyError.ReferencedObjectName)], "ReferencedObject");
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<Column> builder)
    {
        string tableName = nameof(DataContext.Columns);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => new { x.SqlObjectId, x.Ordinal })
            .HasName("PK_" + tableName);

        builder
            .HasOne(x => x.Table)
            .WithMany()
            .HasForeignKey(x => x.SqlObjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex([nameof(Column.ColumnName)], nameof(Column.ColumnName));
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<Integration> builder)
    {
        string tableName = nameof(DataContext.Integrations);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => x.IntegrationId)
            .HasName("PK_" + tableName);

        // Optional Parent Integration
        builder
            .HasOne(x => x.ParentIntegration)
            .WithMany(x => x.ChildIntegrations)
            .HasForeignKey(x => x.ParentIntegrationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);

        builder.HasIndex([nameof(Integration.Name)], nameof(Integration.Name)).IsUnique(true);
    }

    /// <summary></summary>
    public void Configure(EntityTypeBuilder<IntegrationObject> builder)
    {
        string tableName = nameof(DataContext.IntegrationObjects);

        builder
            .ToTable(tableName, Schema)
            .HasKey(x => new { x.IntegrationId, x.SqlObjectId })
            .HasName("PK_" + tableName);

        builder.HasOne(x => x.Integration)
        .WithMany(x => x.IntegrationObjects)
        .HasForeignKey(x => x.IntegrationId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(true);

        builder.HasOne(x => x.SqlObject)
            .WithMany()
            .HasForeignKey(x => x.SqlObjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(true);
    }

}
