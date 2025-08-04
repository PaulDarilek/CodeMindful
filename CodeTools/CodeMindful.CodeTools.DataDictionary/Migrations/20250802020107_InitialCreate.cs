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
            migrationBuilder.CreateTable(
                name: "Catalogs",
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
                columns: table => new
                {
                    IntegrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IntegrationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.IntegrationId);
                });

            migrationBuilder.CreateTable(
                name: "Schemas",
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlObjects", x => x.SqlObjectId);
                });

            migrationBuilder.CreateTable(
                name: "ServerCatalogs",
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
                        principalTable: "Catalogs",
                        principalColumn: "CatalogName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServerCatalogs_Servers_ServerName",
                        column: x => x.ServerName,
                        principalTable: "Servers",
                        principalColumn: "ServerName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Columns",
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
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationObjects",
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
                        principalTable: "Integrations",
                        principalColumn: "IntegrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntegrationObjects_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SqlDependencies",
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
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SqlDependencies_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
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
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId");
                    table.ForeignKey(
                        name: "FK_SqlDependencyErrors_SqlObjects_SqlObjectId",
                        column: x => x.SqlObjectId,
                        principalTable: "SqlObjects",
                        principalColumn: "SqlObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ColumnName",
                table: "Columns",
                column: "ColumnName");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationObjects_SqlObjectId",
                table: "IntegrationObjects",
                column: "SqlObjectId");

            migrationBuilder.CreateIndex(
                name: "Name",
                table: "Integrations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerCatalogs_CatalogName",
                table: "ServerCatalogs",
                column: "CatalogName");

            migrationBuilder.CreateIndex(
                name: "IX_SqlDependencies_ReferencedObjectId",
                table: "SqlDependencies",
                column: "ReferencedObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlDependencies_SqlObjectId",
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
                table: "SqlObjects",
                column: "ObjectName");

            migrationBuilder.CreateIndex(
                name: "QualifiedName",
                table: "SqlObjects",
                columns: new[] { "CatalogName", "SchemaName", "ObjectName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Columns");

            migrationBuilder.DropTable(
                name: "IntegrationObjects");

            migrationBuilder.DropTable(
                name: "Schemas");

            migrationBuilder.DropTable(
                name: "ServerCatalogs");

            migrationBuilder.DropTable(
                name: "SqlDependencies");

            migrationBuilder.DropTable(
                name: "SqlDependencyErrors");

            migrationBuilder.DropTable(
                name: "Integrations");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "SqlObjects");
        }
    }
}
