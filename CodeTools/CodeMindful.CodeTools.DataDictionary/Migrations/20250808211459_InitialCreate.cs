using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeMindful.CodeTools.DataDictionary.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Catalogs",
                schema: "dbo",
                columns: table => new
                {
                    CatalogName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.CatalogName);
                });

            migrationBuilder.CreateTable(
                name: "Integrations",
                schema: "dbo",
                columns: table => new
                {
                    IntegrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IntegrationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentIntegrationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.IntegrationId);
                    table.ForeignKey(
                        name: "FK_Integrations_Integrations_ParentIntegrationId",
                        column: x => x.ParentIntegrationId,
                        principalSchema: "dbo",
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId");
                });

            migrationBuilder.CreateTable(
                name: "Schemas",
                schema: "dbo",
                columns: table => new
                {
                    CatalogName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SchemaName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemas", x => new { x.CatalogName, x.SchemaName });
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                schema: "dbo",
                columns: table => new
                {
                    ServerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ServerName);
                });

            migrationBuilder.CreateTable(
                name: "SqlObjects",
                schema: "dbo",
                columns: table => new
                {
                    SqlObjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SchemaName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ObjectName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ignore = table.Column<bool>(type: "bit", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlObjects", x => x.SqlObjectId);
                });

            migrationBuilder.CreateTable(
                name: "ServerCatalogs",
                schema: "dbo",
                columns: table => new
                {
                    ServerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CatalogName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerCatalogs", x => new { x.ServerName, x.CatalogName });
                    table.ForeignKey(
                        name: "FK_ServerCatalogs_Catalogs_CatalogName",
                        column: x => x.CatalogName,
                        principalSchema: "dbo",
                        principalTable: "Catalogs",
                        principalColumn: "CatalogName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServerCatalogs_Servers_ServerName",
                        column: x => x.ServerName,
                        principalSchema: "dbo",
                        principalTable: "Servers",
                        principalColumn: "ServerName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Columns",
                schema: "dbo",
                columns: table => new
                {
                    SqlObjectId = table.Column<int>(type: "int", nullable: false),
                    Ordinal = table.Column<int>(type: "int", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsNullable = table.Column<bool>(type: "bit", nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Default = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => new { x.SqlObjectId, x.Ordinal });
                    table.ForeignKey(
                        name: "FK_Columns_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
                        principalSchema: "dbo",
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationObjects",
                schema: "dbo",
                columns: table => new
                {
                    IntegrationId = table.Column<int>(type: "int", nullable: false),
                    SqlObjectId = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationObjects", x => new { x.IntegrationId, x.SqlObjectId });
                    table.ForeignKey(
                        name: "FK_IntegrationObjects_Integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalSchema: "dbo",
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntegrationObjects_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
                        principalSchema: "dbo",
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SqlDependencies",
                schema: "dbo",
                columns: table => new
                {
                    SqlDependencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SqlObjectId = table.Column<int>(type: "int", nullable: false),
                    ReferencedObjectId = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlDependencies", x => x.SqlDependencyId);
                    table.ForeignKey(
                        name: "FK_SqlDependencies_SqlObjects_ReferencedObjectId",
                        column: x => x.ReferencedObjectId,
                        principalSchema: "dbo",
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SqlDependencies_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
                        principalSchema: "dbo",
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SqlDependencyErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SqlObjectId = table.Column<int>(type: "int", nullable: false),
                    CatalogName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchemaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferencedCatalogName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferencedSchemaName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferencedObjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferencedObjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlDependencyErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SqlDependencyErrors_SqlObjects_ReferencedObjectId",
                        column: x => x.ReferencedObjectId,
                        principalSchema: "dbo",
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId");
                    table.ForeignKey(
                        name: "FK_SqlDependencyErrors_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
                        principalSchema: "dbo",
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ColumnName",
                schema: "dbo",
                table: "Columns",
                column: "ColumnName");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationObjects_SqlObjectId",
                schema: "dbo",
                table: "IntegrationObjects",
                column: "SqlObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_ParentIntegrationId",
                schema: "dbo",
                table: "Integrations",
                column: "ParentIntegrationId");

            migrationBuilder.CreateIndex(
                name: "Name",
                schema: "dbo",
                table: "Integrations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerCatalogs_CatalogName",
                schema: "dbo",
                table: "ServerCatalogs",
                column: "CatalogName");

            migrationBuilder.CreateIndex(
                name: "IX_SqlDependencies_ReferencedObjectId",
                schema: "dbo",
                table: "SqlDependencies",
                column: "ReferencedObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlDependencies_SqlObjectId",
                schema: "dbo",
                table: "SqlDependencies",
                column: "SqlObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlDependencyErrors_ReferencedObjectId",
                table: "SqlDependencyErrors",
                column: "ReferencedObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlDependencyErrors_SqlObjectId",
                table: "SqlDependencyErrors",
                column: "SqlObjectId");

            migrationBuilder.CreateIndex(
                name: "ObjectName",
                schema: "dbo",
                table: "SqlObjects",
                column: "ObjectName");

            migrationBuilder.CreateIndex(
                name: "QualifiedName",
                schema: "dbo",
                table: "SqlObjects",
                columns: new[] { "CatalogName", "SchemaName", "ObjectName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Columns",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IntegrationObjects",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Schemas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServerCatalogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SqlDependencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SqlDependencyErrors");

            migrationBuilder.DropTable(
                name: "Integrations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Catalogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Servers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SqlObjects",
                schema: "dbo");
        }
    }
}
